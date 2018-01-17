import {
	Component, Input, OnInit, HostBinding, EventEmitter, Output, OnDestroy, ElementRef
} from '@angular/core';
import { Project } from '../../../../models/project';
import { Task } from '../../../../models/task';
import { TimeEntry, DateUtils, CalendarDay } from '../../../../models/calendar';
import { Subscription, Observable } from 'rxjs';
import { ArrayUtils } from '../../../../core/object-utils';
import { TasksService } from '../../../../services/tasks.service';
import { CalendarService } from '../../../../services/calendar.service';
import { NotificationService } from '../../../../core/notification.service';
import { CalendarProjectsService } from '../../calendar-projects.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { Roles } from '../../../../core/auth/permissions';
import { MAX_TIMER_VALUE } from '../../calendar-views/calendar-task/calendar-task.component';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../../../models/user';
import * as moment from 'moment';

export class Time {
	hours: string;
	minutes: string;
	seconds: string;

	constructor(hours: string, minutes: string, seconds?: string) {
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
	}
}

@Component({
	selector: 'ct-entry-time-form',
	templateUrl: 'entry-time-form.component.html'
})

export class EntryTimeFormComponent implements OnInit, OnDestroy {
	@HostBinding('class.ct-entry-time-form') addClass: boolean = true;

	@Input() timeEntry: TimeEntry;
	@Output() closeEntryTimeForm: EventEmitter<void> = new EventEmitter<void>();
	@Output() deleted: EventEmitter<void> = new EventEmitter<void>();
	@Output() timerUpdated: EventEmitter<void> = new EventEmitter<void>();

	actualTime: Time;
	currentTimeEntry: TimeEntry;
	formHeight: number;
	isFocusClassShown: boolean;
	isFormChanged: boolean = false;
	isFromToFormChanged: boolean = false;
	isFromToFormShown: boolean = false;
	isRequestLoading: boolean = false;
	isTimerShown: boolean = false;
	fromTime: Time;
	plannedTime: Time;
	projectList: Project[];
	projectModel: Project;
	selectedDate: Date;
	taskList: Task[];
	taskModel: Task;
	ticks: number;
	timerSubscription: Subscription;
	timerValue: Time;
	toTime: Time;
	userInfo: User;

	private isTasksLoaded: boolean = false;
	private oldActualTime: Time;
	private oldPlannedTime: Time;
	private dayInfo: CalendarDay;
	private defaultProject: Project;
	private totalTrackedTimeForDay: number;
	private totalPlannedTimeForDay: number;

	constructor(private authService: AuthService,
	            private calendarService: CalendarService,
	            private el: ElementRef,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private projectsService: CalendarProjectsService,
	            private route: ActivatedRoute,
	            private tasksService: TasksService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
		});

		this.currentTimeEntry = new TimeEntry(this.timeEntry);
		this.actualTime = this.splitTime(this.timeEntry.time);
		this.plannedTime = this.splitTime(this.timeEntry.plannedTime);
		this.cashTime();
		this.closeFromToForm();
		this.selectedDate = this.currentTimeEntry.date;
		this.loadProjects();

		if (this.currentTimeEntry.timeTimerStart > 0) {
			this.startTimer(DateUtils.getSecondsFromStartDay() - this.currentTimeEntry.timeTimerStart);
		}
		setTimeout(() => {
			if (this.currentTimeEntry.isFromToShow) {
				this.fillFromToForm();
			}
		}, 0);

		this.getFormHeight();
	}

	// PROJECT

	isArchivedProjectShown(): boolean {
		return this.currentTimeEntry.id && !this.currentTimeEntry.isProjectActive && !this.projectModel;
	}

	projectOnChange(projectModel: Project): void {
		this.isFormChanged = true;
		this.currentTimeEntry.projectId = projectModel.id;
		this.currentTimeEntry.projectName = projectModel.name;
		this.currentTimeEntry.projectName = projectModel.name;
		this.currentTimeEntry.color = projectModel.color;
		this.loadTasks(projectModel.id);
	}

	// TASK

	isArchivedTaskShown(): boolean {
		return this.currentTimeEntry.id && !this.currentTimeEntry.isTaskTypeActive && !this.taskModel;
	}

	taskOnChange(taskModel: Task): void {
		this.isFormChanged = true;
		this.currentTimeEntry.taskTypesId = taskModel.id;
		this.currentTimeEntry.taskName = taskModel.name;
	}

	// DESCRIPTION

	descriptionOnChange(): void {
		this.getFormHeight();
		this.isFormChanged = true;
	}

	// TIMER

	canActivateTimer(): boolean {
		return this.isTimerShown
			|| (!this.calendarService.isTimerActivated
			&& this.isToday()
			&& !!this.currentTimeEntry.projectId
			&& !!this.currentTimeEntry.taskTypesId);
	}

	isToday(): boolean {
		return DateUtils.isToday(new Date(this.timeEntry.date));
	}

	startTimer(timeTimerStart?: number): void {
		this.isTimerShown = true;
		timeTimerStart = timeTimerStart ? timeTimerStart + this.currentTimeEntry.time : this.currentTimeEntry.time;
		let timer = Observable.timer(0, 1000);
		this.timerSubscription = timer.subscribe(() => {
			this.ticks = timeTimerStart++;
			this.timerValue = this.getTimeString(this.ticks);
		});
	}

	stopTimer(): void {
		this.timerSubscription.unsubscribe();
		this.isTimerShown = false;
	}

	toggleTimer(): void {
		if (!this.isToday() || !this.isTimerValid()) {
			return;
		}
		this.calendarService.isTimerActivated = true;
		if (!this.currentTimeEntry.id) {
			this.currentTimeEntry.timeTimerStart = DateUtils.getSecondsFromStartDay();
			this.isFromToFormShown = false;
			this.submit();
			return;
		}
		this.saveTimerStatus().then((err: any) => {
			if (err) {
				return;
			}
			this.timerUpdated.emit();
			if (this.isTimerShown) {
				this.stopTimer();
			} else {
				this.closeFromToForm();
				this.startTimer();
			}
			this.calendarService.isTimerActivated = this.isTimerShown;
			this.calendarService.setDefaultProject(this.projectModel);
			this.closeEntryTimeForm.emit();
		});
	}

	private isTimerValid(): boolean {
		if (!this.isCurrentTrackedTimeValid(true)) {
			this.notificationService.danger('Total actual time can\'t be more than 24 hours');
			return false;
		}

		return true;
	}

	private saveTimerStatus(): Promise<any> {
		if (!this.isTimerShown) {
			this.currentTimeEntry.isFromToShow = false;
			this.currentTimeEntry.timeFrom = null;
			this.currentTimeEntry.timeTimerStart = DateUtils.getSecondsFromStartDay();
			this.currentTimeEntry.timeTo = null;
		} else {
			this.currentTimeEntry.time = this.ticks;
			this.currentTimeEntry.timeTimerStart = -1;
			this.actualTime = this.splitTime(this.currentTimeEntry.time);
		}

		this.currentTimeEntry.date = DateUtils.convertMomentToUTC(moment(this.currentTimeEntry.date));
		return this.calendarService.Put(this.currentTimeEntry, this.currentTimeEntry.id.toString())
			.toPromise().then(
				() => {
					this.saveTimeEntry(this.currentTimeEntry);
					if (this.isTimerShown) {
						this.notificationService.success('Timer has stopped.');
					} else {
						this.notificationService.success('Timer has been successfully created.');
					}
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status');
					return error;
				});
	}

	// FROM-TO FORM

	isFromToFormValueValid(): boolean {
		return this.convertFormValueToSeconds(this.toTime) > this.convertFormValueToSeconds(this.fromTime);
	}

	openFromToForm(): void {
		this.isFromToFormShown = true;
	}

	closeFromToForm(): void {
		this.isFromToFormShown = false;
		this.fromTime = new Time('00', '00');
		this.toTime = new Time('00', '00');
	}

	validateFromToForm(): void {
		this.isFormChanged = true;
		this.isFromToFormChanged = true;
		this.toTime = this.getMax(this.fromTime, this.toTime);
		this.setActualTime();
		this.actualTime = this.splitTime(this.currentTimeEntry.time);
		this.cashTime();
	}

	private getMax(fromTime: Time, toTime: Time): Time {
		if (this.convertFormValueToSeconds(fromTime) > this.convertFormValueToSeconds(toTime)) {
			return new Time(fromTime.hours, fromTime.minutes);
		}
		return toTime;
	}

	// TRACKING TIME

	actualTimeOnChange(): void {
		if (this.oldActualTime && this.oldActualTime.hours === this.actualTime.hours && this.oldActualTime.minutes === this.actualTime.minutes) {
			return;
		}

		this.closeFromToForm();
		this.isFormChanged = true;
		this.currentTimeEntry.time = this.convertFormValueToSeconds(this.actualTime);
		this.currentTimeEntry.timeFrom = null;
		this.currentTimeEntry.timeTo = null;
		this.cashTime();
	}

	private cashTime(): void {
		this.oldActualTime = new Time(this.actualTime.hours, this.actualTime.minutes);
		this.oldPlannedTime = new Time(this.plannedTime.hours, this.plannedTime.minutes);
	}

	plannedTimeOnChange(): void {
		this.isFormChanged = true;
		this.currentTimeEntry.plannedTime = this.convertFormValueToSeconds(this.plannedTime);
	}

	// SUBMIT TIMEENTRY

	closeForm(): void {
		this.closeEntryTimeForm.emit();
	}

	submit(): void {
		if (!this.isSubmitDataValid()) {
			return;
		}

		let submitObservable: Observable<any>;
		let isNewTimeEntry: boolean;

		this.currentTimeEntry.date = DateUtils.convertMomentToUTC(moment(this.currentTimeEntry.date));
		this.currentTimeEntry.isFromToShow = this.isFromToFormShown && this.isFromToFormValueValid();
		this.currentTimeEntry.memberId = this.impersonationService.impersonationId || this.authService.getAuthUser().id;

		if (this.currentTimeEntry.id) {
			submitObservable = this.calendarService.Put(this.currentTimeEntry, this.currentTimeEntry.id.toString());
			isNewTimeEntry = false;
		} else {
			submitObservable = this.calendarService.Post(this.currentTimeEntry);
			isNewTimeEntry = true;
		}

		this.isRequestLoading = true;
		submitObservable.toPromise().then(
			() => {
				this.isRequestLoading = false;
				this.saveTimeEntry(this.currentTimeEntry);
				this.calendarService.setDefaultProject(this.projectModel);
				if (!this.currentTimeEntry.id) {
					this.notificationService.success('New Time Entry has been successfully created.');
				} else {
					this.notificationService.success('Time Entry has been successfully changed.');
				}
				this.calendarService.isTimerActivated = this.isTimerShown;
				if (isNewTimeEntry) {
					this.calendarService.timeEntriesUpdated.emit();
				}
				this.closeEntryTimeForm.emit();
			},
			error => {
				this.isRequestLoading = false;
				if (!this.currentTimeEntry.id) {
					this.notificationService.danger('Error creating Time Entry');
				} else {
					this.notificationService.danger('Error changing Time Entry');
				}
			});
	}

	isSubmitDataValid(): boolean {
		if (!this.isCurrentTrackedTimeValid()) {
			this.notificationService.danger('Total actual time can\'t be more than 24 hours');
			return false;
		}
		if (!this.isPlannedTimeValid()) {
			this.notificationService.danger('Total planned time can\'t be more than 24 hours');
			return false;
		}
		if (this.currentTimeEntry.isFromToShow && !this.isFromToTimeValid()) {
			this.notificationService.danger('Selected time period already exists');
			return false;
		}

		return true;
	}

	ngOnDestroy() {
		if (this.timerSubscription) {
			this.timerSubscription.unsubscribe();
		}
	}

	isCurrentTrackedTimeValid(isStrongValidation?: boolean): boolean {
		this.setDayInfo();
		if (isStrongValidation) {
			return this.totalTrackedTimeForDay - this.timeEntry.time + this.currentTimeEntry.time < MAX_TIMER_VALUE;
		} else {
			return this.totalTrackedTimeForDay - this.timeEntry.time + this.currentTimeEntry.time <= MAX_TIMER_VALUE;
		}
	}

	isPlannedTimeValid(): boolean {
		this.setDayInfo();
		return this.totalPlannedTimeForDay - this.timeEntry.plannedTime + this.currentTimeEntry.plannedTime < MAX_TIMER_VALUE;
	}

	private getFormHeight(): void {
		setTimeout(() => this.formHeight = this.el.nativeElement.offsetHeight, 0);
	}

	private formatTime(time: number): string {
		return (time >= 0 && time < 10) ? '0' + time : time + '';
	}

	private loadProjects(): void {
		this.projectsService.getProjects(true).subscribe((res) => {
			this.projectList = this.removeNonActiveProjects(res);
			this.projectList = this.filterProjects(this.projectList);

			this.defaultProject = this.calendarService.defaultProject;
			if (this.defaultProject) {
			this.projectModel = ArrayUtils.findByProperty(this.projectList, 'id',
				this.currentTimeEntry.projectId || this.defaultProject.id || this.userInfo.defaultProjectId);
			} else {
				this.projectModel = ArrayUtils.findByProperty(this.projectList, 'id',
					this.currentTimeEntry.projectId || this.userInfo.defaultProjectId);
			}

			if (this.projectList.length === 1) {
				this.currentTimeEntry.projectName = this.projectList[0].name;
				this.currentTimeEntry.projectId = this.projectList[0].id;
				this.currentTimeEntry.color = this.projectList[0].color;
				this.loadTasks(this.timeEntry.projectId);
			}

			if (this.projectModel) {
				this.timeEntry.projectId = this.projectModel.id;
				this.timeEntry.projectName = this.projectModel.name;
				this.timeEntry.color = this.projectModel.color;
			} else {
				this.loadTasks(this.timeEntry.projectId);
			}
		});
	}

	private loadTasks(projectId?: number): void {
		this.tasksService.getActiveTasks(projectId).subscribe((res) => {
			this.taskList = this.filterTasks(res.data);
			this.taskModel = ArrayUtils.findByProperty(this.taskList, 'id', this.currentTimeEntry.taskTypesId || this.userInfo.defaultTaskId);

			if (!this.isTasksLoaded && this.taskModel) {
				this.timeEntry.taskTypesId = this.taskModel.id;
				this.timeEntry.taskName = this.taskModel.name;
			}

			this.isTasksLoaded = true;
		});
	}

	private filterTasks(tasks: Task[]): Task[] {
		let filteredTasks: Task[] = [];
		let isAdded: boolean = false;
		tasks.forEach((task1, index1) => {
			isAdded = false;
			if (task1.projectId) {
				filteredTasks.push(task1);
			} else {
				tasks.forEach((task2, index2) => {
					if (task2.projectId && task1.name.toLowerCase() === task2.name.toLowerCase() && index1 !== index2) {
						isAdded = true;
					}
				});
				if (!isAdded) {
					filteredTasks.push(task1);
				}
			}
		});
		return filteredTasks;
	}

	private saveTimeEntry(timeEntry: TimeEntry): void {
		for (let prop in this.timeEntry) {
			this.timeEntry[prop] = timeEntry[prop];
		}
	}

	private setActualTime(): void {
		this.currentTimeEntry.timeFrom = this.convertFormValueToSeconds(this.fromTime);
		this.currentTimeEntry.timeTo = this.convertFormValueToSeconds(this.toTime);
		this.currentTimeEntry.time = this.convertFormValueToSeconds(this.toTime) -
			this.convertFormValueToSeconds(this.fromTime);
	}

	private convertFormValueToSeconds(time: Time): number {
		return +time.hours * 3600 + (+time.minutes) * 60;
	}

	private splitTime(time: number = 0): Time {
		let hours = Math.floor(time / 3600 >> 0);
		let minutes = Math.floor(time / 60 >> 0) - hours * 60;
		return new Time(this.formatTime(hours), this.formatTime(minutes));
	}

	private getTimeString(time: number): Time {
		let h = Math.floor(time / 3600 >> 0);
		let m = Math.floor(time / 60 >> 0) - h * 60;
		let s = time - m * 60 - h * 3600;
		return new Time(('00' + h).slice(-2), ('00' + m).slice(-2), ('00' + s).slice(-2));
	}

	private fillFromToForm(): void {
		this.isFromToFormShown = true;
		this.fromTime = this.splitTime(this.currentTimeEntry.timeFrom);
		this.toTime = this.splitTime(this.currentTimeEntry.timeTo);
	}

	private removeNonActiveProjects(projectList: Project[]): Project[] {
		let isUserAdmin = this.authService.getAuthUser().role === Roles.admin;
		if (isUserAdmin) {
			return projectList.filter(project => project.isActive === true);
		} else {
			return projectList.filter(project => project.isActive === true);
		}
	}

	private filterProjects(projectList: Project[]): Project[] {
		let filteredProjects = this.projectsService.filteredProjects;
		if (filteredProjects.length) {
			projectList = projectList.filter((project) => {
				return filteredProjects.indexOf(project.id) > -1;
			});
		}
		return projectList;
	}

	private setDayInfo(date?: Date): void {
		this.dayInfo = this.calendarService.getDayInfoByDate(date || this.timeEntry.date);
		this.totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(this.dayInfo, 'time');
		this.totalPlannedTimeForDay = this.calendarService.getTotalTimeForDay(this.dayInfo, 'plannedTime');
	}

	isFromToTimeValid(): boolean {
		return this.dayInfo.timeEntries
			.filter((timeEntry: TimeEntry) => timeEntry.isFromToShow && timeEntry.id !== this.currentTimeEntry.id)
			.every((timeEntry: TimeEntry) => {
				return timeEntry.timeFrom >= this.currentTimeEntry.timeTo || this.currentTimeEntry.timeFrom >= timeEntry.timeTo;
			});
	}
}
