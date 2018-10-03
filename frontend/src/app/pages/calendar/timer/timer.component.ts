import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { CalendarDay, DateUtils, Time, TimeEntry } from '../../../models/calendar';
import { Project } from '../../../models/project';
import { Task } from '../../../models/task';
import { User } from '../../../models/user';
import { AuthService } from '../../../core/auth/auth.service';
import { CalendarService } from '../../../services/calendar.service';
import { CalendarProjectsService } from '../calendar-projects.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { NotificationService } from '../../../core/notification.service';
import { TasksService } from '../../../services/tasks.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';
import { ArrayUtils } from '../../../core/object-utils';

export const MAX_TIMER_VALUE = 86399;

@Component({
	selector: 'ct-timer',
	templateUrl: 'timer.component.html'
})

export class TimerComponent implements OnInit, OnDestroy {
	@Input() calendar: CalendarDay[];

	defaultProject: Project;
	defaultTask: Task;
	timeEntry: TimeEntry;
	ticks: number = 0;
	timerSubscription: Subscription;
	timerValue: Time;
	userInfo: User;

	constructor(private authService: AuthService,
	            private calendarService: CalendarService,
	            private impersonationService: ImpersonationService,
	            private loadingService: LoadingMaskService,
	            private notificationService: NotificationService,
	            private projectsService: CalendarProjectsService,
	            private route: ActivatedRoute,
	            private tasksService: TasksService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
		});

		this.calendarService.getTimer().subscribe((res) => {
			this.timeEntry = res || new TimeEntry({
				date: DateUtils.formatDateToString(new Date()),
				memberId: this.impersonationService.impersonationId || this.authService.authUser.id
			});

			this.checkTimer();
			if (this.isTimerExist()) {
				this.timerValue = this.convertSecondsToTimeFormat(this.timeEntry.timeValues.timeActual);
			}
			if (this.isTimerActivated()) {
				this.startTimerFront();
			}

			this.loadProjects();
		});
	}

	isTimerActivated(): boolean {
		return !!this.timeEntry
			&& this.timeEntry.timeOptions.timeTimerStart !== 0
			&& this.timeEntry.timeOptions.timeTimerStart !== -1
			&& this.timeEntry.timeValues.timeActual === 0;
	}

	isTimerExist(): boolean {
		return !!this.timeEntry
			&& this.timeEntry.timeOptions.timeTimerStart !== 0
			&& this.timeEntry.timeOptions.timeTimerStart !== -1;
	}

	getButtonTitle(): string {
		if (!this.isTimerExist()) {
			return 'Start timer with default project and default task'
		}
		if (this.isTimerActivated()) {
			return 'Pause timer'
		}
		if (!this.isTimerActivated()) {
			return 'Resume timer'
		}
	}

	getTotalTime(timeField: string): string {
		let time = 0;

		this.calendar.forEach((day: CalendarDay) => {
			time += this.calendarService.getTotalTimeForDay(day, timeField);
		});

		return this.convertSecondsToStringFormat(time);
	}

	private convertSecondsToStringFormat(time: number): string {
		let h = Math.floor(time / 3600 >> 0);
		let m = Math.floor(time / 60 >> 0) - h * 60;
		return ('00' + h).slice(-2) + ':' + ('00' + m).slice(-2);
	}

	private convertSecondsToTimeFormat(time: number): Time {
		let h = Math.floor(time / 3600 >> 0);
		let m = Math.floor(time / 60 >> 0) - h * 60;
		let s = time - m * 60 - h * 3600;
		return new Time(('00' + h).slice(-2), ('00' + m).slice(-2), ('00' + s).slice(-2));
	}

	private roundTime(time: number): number {
		return time - time % 60;
	}

	// TIMER COMMANDS

	startTimer(): void {
		if (!this.isTimerValid()) {
			return;
		}

		let promise: Promise<any>;
		if (!this.timeEntry.id) {
			promise = this.createTimer()
		} else {
			promise = this.resumeTimer();
		}

		promise.then((err: any) => {
			if (err) {
				return;
			}

			this.startTimerFront();
		})
	}

	startTimerFront(): void {
		let timer = Observable.timer(0, 1000);
		this.timerSubscription = timer.subscribe(() => {
			this.ticks = DateUtils.getSecondsFromStartDay(true) - this.timeEntry.timeOptions.timeTimerStart
				+ this.timeEntry.timeValues.timeActual;
			this.timerValue = this.convertSecondsToTimeFormat(this.ticks);
			this.checkTimer();
		});
	}

	stopTimer(completely?: boolean): void {
		let promise: Promise<any>;
		if (!completely) {
			promise = this.pauseTimer();
		} else {
			promise = this.deleteTimer();
		}

		promise.then((err: any) => {
			if (err) {
				return;
			}

			this.stopTimerFront(completely);
		})
	}

	stopTimerFront(completely?: boolean): void {
		this.ticks = 0;
		this.timerSubscription.unsubscribe();
		this.timerValue = this.convertSecondsToTimeFormat(completely ? 0 : this.timeEntry.timeValues.timeActual);
	}

	toggleTimer(): void {
		if (!this.isTimerActivated()) {
			this.startTimer();
		} else {
			this.stopTimer();
		}
	}

	createTimer(): Promise<void> {
		let timeEntry = this.createDefaultTimeEntry();
		timeEntry.timeOptions.timeTimerStart = DateUtils.getSecondsFromStartDay(true);

		return this.calendarService.Post(timeEntry)
			.toPromise().then(
				(timeEntry) => {
					this.timeEntry = new TimeEntry(timeEntry);
					this.notificationService.success('Timer has been successfully created.');
				},
				() => {
					this.notificationService.danger('Error creating Timer.');
				});
	}

	resumeTimer(): Promise<void> {
		let timeEntry = Object.assign({}, this.timeEntry);

		timeEntry.timeOptions.timeTimerStart = DateUtils.getSecondsFromStartDay(true) - this.timeEntry.timeValues.timeActual;
		timeEntry.timeValues.timeActual = 0;

		return this.calendarService.Put(timeEntry, timeEntry.id.toString())
			.toPromise().then(
				() => {
					this.notificationService.success('Timer has been resumed.');
					this.saveTimeEntry(timeEntry);
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	pauseTimer(): Promise<void> {
		let timeEntry = Object.assign({}, this.timeEntry);

		timeEntry.timeOptions.timeTimerStart = DateUtils.getSecondsFromStartDay(true);
		timeEntry.timeValues.timeActual = this.ticks;

		return this.calendarService.Put(timeEntry, timeEntry.id.toString())
			.toPromise().then(
				() => {
					this.notificationService.success('Timer has paused.');
					this.saveTimeEntry(timeEntry);
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	deleteTimer(): Promise<void> {
		let timeEntry = Object.assign({}, this.timeEntry);

		timeEntry.timeOptions = {
			isFromToShow: true,
			timeTimerStart: 0
		};
		let secondsFromStartDay = this.roundTime(DateUtils.getSecondsFromStartDay(false));
		let roundTicks = (this.ticks < 60) ? 60 : this.roundTime(this.ticks);
		timeEntry.timeValues = {
			timeActual: roundTicks,
			timeEstimated: 0,
			timeFrom: Math.max(secondsFromStartDay - roundTicks, 0),
			timeTo: Math.max(secondsFromStartDay - roundTicks, 0) + roundTicks
		};

		return this.calendarService.Put(timeEntry, timeEntry.id.toString())
			.toPromise().then(
				() => {
					this.notificationService.success('Timer has stopped.');
					this.timeEntry = this.createDefaultTimeEntry();
					this.calendarService.timeEntriesUpdated.emit();
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	// CHECK TIMER

	checkTimer(): void {
		let errorMessage: string;

		let isTimeEntryAvailable: boolean;
		if (this.userInfo.isAdmin || this.timeEntry.isUserManagerOnProject) {
			isTimeEntryAvailable = true;
		} else {
			isTimeEntryAvailable = !this.timeEntry.isLocked
				&& this.timeEntry.isProjectActive && this.timeEntry.isTaskTypeActive;
		}

		if (!DateUtils.isToday(this.timeEntry.date)) {
			errorMessage = 'Selected time period should be within one day. Timer has stopped.'
		}
		if (!errorMessage && !isTimeEntryAvailable) {
			errorMessage = 'Timer has stopped, because Time Entry is locked.';
		}
		if (!errorMessage && !this.isTrackedTimeValid()) {
			errorMessage = 'Total actual time should be less than 24 hours. Timer has stopped.';
		}

		if (errorMessage) {
			this.autoStopTimer(errorMessage);
		}
	}

	autoStopTimer(errorMessage: string): void {
		let timeEntry = new TimeEntry(this.timeEntry);
		let totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(this.getDayInfo(), 'timeActual');
		let finalTimerValue = this.limitTimerValue(MAX_TIMER_VALUE - (totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual));
		let timeTimerStart = this.limitTimerValue(this.timeEntry.timeOptions.timeTimerStart - this.timeEntry.timeValues.timeActual
			- new Date().getTimezoneOffset() * 60); // locale format

		timeEntry.timeOptions = {
			isFromToShow: true,
			timeTimerStart: 0
		};
		timeEntry.timeValues = {
			timeActual: Math.min(MAX_TIMER_VALUE - timeTimerStart, finalTimerValue),
			timeEstimated: 0,
			timeFrom: timeTimerStart,
			timeTo: Math.min(MAX_TIMER_VALUE - timeTimerStart, finalTimerValue) + timeTimerStart
		};

		this.calendarService.Put(timeEntry, timeEntry.id.toString())
			.subscribe(() => {
					this.notificationService.danger(errorMessage);
					this.timeEntry = this.createDefaultTimeEntry();
					this.calendarService.timeEntriesUpdated.emit();
					this.stopTimerFront();
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	private getDayInfo(): CalendarDay {
		return this.calendarService.getDayInfoByDate(this.timeEntry.date);
	}

	private isTimerValid(): boolean {
		if (!this.defaultProject) {
			this.notificationService.danger('Default project doesn\'t exist.');
			return false;
		}
		if (!this.defaultTask) {
			this.notificationService.danger('Default task doesn\'t exist.');
			return false;
		}
		if (!this.isCurrentTrackedTimeValid()) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}

		return true;
	}

	private isTrackedTimeValid(): boolean {
		let totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(this.getDayInfo(), 'timeActual');
		return totalTrackedTimeForDay + this.ticks - this.timeEntry.timeValues.timeActual < MAX_TIMER_VALUE;
	}

	private limitTimerValue(time: number): number {
		return Math.min(Math.max(time, 0), MAX_TIMER_VALUE);
	}

	ngOnDestroy() {
		if (this.timerSubscription) {
			this.timerSubscription.unsubscribe();
		}
	}

	// GENERAL

	private createDefaultTimeEntry(): TimeEntry {
		return new TimeEntry({
			color: this.defaultProject.color,
			date: DateUtils.formatDateToString(new Date()),
			memberId: this.impersonationService.impersonationId || this.authService.authUser.id,
			projectId: this.defaultProject.id,
			projectName: this.defaultProject.name,
			taskTypesId: this.defaultTask.id,
			taskName: this.defaultTask.name
		});
	}

	private isCurrentTrackedTimeValid(): boolean {
		let dayInfo = this.calendarService.getDayInfoByDate(this.timeEntry.date);
		let totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'timeActual');

		return totalTrackedTimeForDay + (this.ticks || this.timeEntry.timeValues.timeActual) < MAX_TIMER_VALUE;
	}

	private loadProjects(): void {
		this.projectsService.getProjects(true).subscribe((projectList) => {
			this.defaultProject = ArrayUtils.findByProperty(projectList, 'id', this.userInfo.defaultProjectId);

			if (this.defaultProject) {
				this.timeEntry.projectId = this.defaultProject.id;
				this.timeEntry.projectName = this.defaultProject.name;
				this.timeEntry.color = this.defaultProject.color;
				this.loadTasks(this.timeEntry.projectId);
			}
		});
	}

	private loadTasks(projectId?: number): void {
		this.tasksService.getActiveTasks(projectId)
			.subscribe((res) => {
				this.defaultTask = ArrayUtils.findByProperty(res.data, 'id', this.userInfo.defaultTaskId);

				if (this.defaultTask) {
					this.timeEntry.taskTypesId = this.defaultTask.id;
					this.timeEntry.taskName = this.defaultTask.name;
				}
			});
	}

	private saveTimeEntry(timeEntry: TimeEntry): void {
		for (let prop in this.timeEntry) {
			this.timeEntry[prop] = timeEntry[prop];
		}
	}
}
