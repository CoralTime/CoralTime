import { Component, Input, OnInit } from '@angular/core';
import { CalendarDay, DateUtils, Time, TimeEntry } from '../../../models/calendar';
import { AuthService } from '../../../core/auth/auth.service';
import { CalendarService } from '../../../services/calendar.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { NotificationService } from '../../../core/notification.service';
import { MAX_TIMER_VALUE } from '../calendar-views/calendar-task/calendar-task.component';
import { User } from '../../../models/user';
import { ArrayUtils } from '../../../core/object-utils';
import { CalendarProjectsService } from '../calendar-projects.service';
import { TasksService } from '../../../services/tasks.service';
import { Project } from '../../../models/project';
import { Task } from '../../../models/task';

@Component({
	selector: 'ct-timer',
	templateUrl: 'timer.component.html'
})

export class TimerComponent implements OnInit {
	@Input() calendar: CalendarDay[];

	defaultProject: Project;
	defaultTask: Task;
	timeEntry: TimeEntry;
	ticks: number = 0;
	timerSubscription: Subscription;
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

			if (this.isTimerActivated()) {
				let timer = Observable.timer(0, 1000);
				this.timerSubscription = timer.subscribe(() => {
					this.ticks = DateUtils.getSecondsFromStartDay(true) - this.timeEntry.timeOptions.timeTimerStart
						+ this.timeEntry.timeValues.timeActual;
				})
			}

			this.loadProjects();
		});
	}

	isTimerActivated(): boolean {
		return !!this.timeEntry && this.timeEntry.timeOptions.timeTimerStart > 0
			&& this.timeEntry.timeValues.timeActual === 0;
	}

	isTimerExist(): boolean {
		return !!this.timeEntry && this.timeEntry.timeOptions.timeTimerStart > 0;
	}

	getTotalTime(timeField: string): Time {
		let time = 0;

		this.calendar.forEach((day: CalendarDay) => {
			time += this.calendarService.getTotalTimeForDay(day, timeField);
		});

		if (this.timeEntry && timeField === 'timeActual') {
			time += this.timeEntry.timeValues.timeActual + this.ticks;
		}

		return this.convertSecondsToTimeFormat(time);
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

			let timer = Observable.timer(0, 1000);
			this.timerSubscription = timer.subscribe(() => {
				this.ticks = DateUtils.getSecondsFromStartDay(true) - this.timeEntry.timeOptions.timeTimerStart
					+ this.timeEntry.timeValues.timeActual;
			});
		})
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

			this.ticks = 0;
			this.timerSubscription.unsubscribe();
		})
	}

	// canActivateTimer(): boolean {
	// 	return this.isTimerShown
	// 		|| (!this.calendarService.isTimerActivated
	// 			&& !!this.currentTimeEntry.projectId
	// 			&& !!this.currentTimeEntry.taskTypesId);
	// }

	toggleTimer(): void {
		// if (!this.isTimerValid()) {
		// 	return;
		// }

		if (!this.isTimerActivated()) {
			this.startTimer();
		} else {
			this.stopTimer();
		}
	}

	private isTimerValid(): boolean {
		// if (!this.isCurrentTrackedTimeValid(true)) {
		// 	this.notificationService.danger('Total actual time should be less than 24 hours.');
		// 	return false;
		// }

		return true;
	}

	createTimer(): Promise<void> {
		// if (!this.isSubmitDataValid()) {
		// 	return;
		// }

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
			timeTimerStart: -1
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

	// private isCurrentTrackedTimeValid(isStrongValidation?: boolean): boolean {
	// 	this.setDayInfo();
	// 	if (isStrongValidation) {
	// 		return this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual
	// 			+ this.currentTimeEntry.timeValues.timeActual < MAX_TIMER_VALUE;
	// 	} else {
	// 		return this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual
	// 			+ this.currentTimeEntry.timeValues.timeActual <= MAX_TIMER_VALUE;
	// 	}
	// }

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
