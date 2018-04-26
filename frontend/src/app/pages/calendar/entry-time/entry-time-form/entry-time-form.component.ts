import {
	Component, Input, OnInit, HostBinding, EventEmitter, Output, OnDestroy, ElementRef
} from '@angular/core';
import { Project } from '../../../../models/project';
import { Task } from '../../../../models/task';
import { TimeEntry, DateUtils, CalendarDay, Time } from '../../../../models/calendar';
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
import { SelectItem } from 'primeng/primeng';

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

	currentTimeEntry: TimeEntry;
	formHeight: number;
	isFocusClassShown: boolean;
	isFormChanged: boolean;
	isFromToFormChanged: boolean;
	isFromToFormFocus: boolean;
	isRequestLoading: boolean;
	isTimerShown: boolean;
	projectList: Project[];
	projectModel: Project;
	taskList: Task[];
	taskModel: Task;
	ticks: number;
	timeActual: string;
	timeEstimated: string;
	timeFrom: string = '00:00';
	timeTo: string = '00:00';
	timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
	timerSubscription: Subscription;
	timerValue: Time;
	userInfo: User;
	afternoonList: SelectItem[] = [
		{
			label: 'AM',
			value: 0
		},
		{
			label: 'PM',
			value: 1
		}
	];
	afternoonModel: SelectItem = this.afternoonList[0];

	private isTasksLoaded: boolean = false;
	private dayInfo: CalendarDay;
	private defaultProject: Project;
	private totalTrackedTimeForDay: number;
	private totalEstimatedTimeForDay: number;

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
		this.timeActual = this.convertTimeToString(this.timeEntry.timeValues.timeActual);
		this.timeEstimated = this.convertTimeToString(this.timeEntry.timeValues.timeEstimated);

		if (this.currentTimeEntry.timeOptions.isFromToShow) {
			this.fillFromToForm();
		}

		this.loadProjects();

		if (this.timeEntry.timeOptions.timeTimerStart && this.timeEntry.timeOptions.timeTimerStart !== -1) {
			this.startTimer();
		}

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
		return DateUtils.isToday(this.timeEntry.date);
	}

	startTimer(): void {
		this.isTimerShown = true;

		let timer = Observable.timer(0, 1000);
		this.timerSubscription = timer.subscribe(() => {
			this.ticks = DateUtils.getSecondsFromStartDay(true) - this.currentTimeEntry.timeOptions.timeTimerStart
				+ this.currentTimeEntry.timeValues.timeActual;
			this.timerValue = this.convertSecondsToTimeFormat(this.ticks);
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
			this.currentTimeEntry.timeOptions.timeTimerStart = DateUtils.getSecondsFromStartDay(true);
			this.currentTimeEntry.timeOptions.isFromToShow = false;
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
			this.projectsService.setDefaultProject(this.projectModel);
			this.closeEntryTimeForm.emit();
		});
	}

	private isTimerValid(): boolean {
		if (!this.isCurrentTrackedTimeValid(true)) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}

		return true;
	}

	private saveTimerStatus(): Promise<any> {
		if (!this.isTimerShown) {
			this.currentTimeEntry.timeOptions = {
				isFromToShow: false,
				timeTimerStart: DateUtils.getSecondsFromStartDay(true)
			};
			this.currentTimeEntry.timeValues.timeFrom = null;
			this.currentTimeEntry.timeValues.timeTo = null;
		} else {
			this.currentTimeEntry.timeOptions = {
				isFromToShow: true,
				timeTimerStart: -1
			};
			this.currentTimeEntry.timeValues = {
				timeActual: this.ticks,
				timeEstimated: this.currentTimeEntry.timeValues.timeEstimated,
				timeFrom: Math.max(DateUtils.getSecondsFromStartDay(false) - this.ticks, 0),
				timeTo: Math.max(DateUtils.getSecondsFromStartDay(false) - this.ticks, 0) + this.ticks
			};
			this.timeActual = this.convertTimeToString(this.currentTimeEntry.timeValues.timeActual);
		}

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
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	// FROM-TO FORM

	openFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = true;
	}

	closeFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = false;
		this.timeFrom = '00:00';
		this.timeTo = '00:00';
	}

	validateFromToForm(timeFrom: string, timeTo: string): void {
		this.isFormChanged = true;
		this.isFromToFormChanged = true;
		this.isFromToFormFocus = false;

		this.timeTo = this.getMax(timeFrom, timeTo);
		this.setTimeActual();
		this.timeActual = this.convertTimeToString(this.currentTimeEntry.timeValues.timeActual);
	}

	private isFromToFormValueValid(): boolean {
		return this.convertFormValueToSeconds(this.timeTo) > this.convertFormValueToSeconds(this.timeFrom);
	}

	private fillFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = true;
		this.timeFrom = this.convertTimeToString(this.currentTimeEntry.timeValues.timeFrom);
		this.timeTo = this.convertTimeToString(this.currentTimeEntry.timeValues.timeTo);
	}

	private getMax(timeFrom: string, timeTo: string): string {
		if (this.convertFormValueToSeconds(timeFrom) > this.convertFormValueToSeconds(timeTo)) {
			return timeFrom;
		}

		return timeTo;
	}

	// TRACKING TIME

	timeActualOnChange(): void {
		this.closeFromToForm();
		this.isFormChanged = true;
		this.currentTimeEntry.timeValues.timeActual = this.convertFormValueToSeconds(this.timeActual);
		this.currentTimeEntry.timeValues.timeFrom = null;
		this.currentTimeEntry.timeValues.timeTo = null;
	}

	timeEstimatedOnChange(): void {
		this.isFormChanged = true;
		this.currentTimeEntry.timeValues.timeEstimated = this.convertFormValueToSeconds(this.timeEstimated);
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

		this.currentTimeEntry.timeOptions.isFromToShow = this.currentTimeEntry.timeOptions.isFromToShow && this.isFromToFormValueValid();
		this.currentTimeEntry.memberId = this.impersonationService.impersonationId || this.authService.authUser.id;

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
				this.calendarService.isTimerActivated = this.isTimerShown;
				this.projectsService.setDefaultProject(this.projectModel);

				if (!this.currentTimeEntry.id) {
					this.notificationService.success('New Time Entry has been successfully created.');
				} else {
					this.notificationService.success('Time Entry has been successfully changed.');
				}
				if (isNewTimeEntry) {
					this.calendarService.timeEntriesUpdated.emit();
				}

				this.closeEntryTimeForm.emit();
			},
			error => {
				this.isRequestLoading = false;

				if (!this.currentTimeEntry.id) {
					this.notificationService.danger('Error creating Time Entry.');
				} else {
					this.notificationService.danger('Error changing Time Entry.');
				}
			});
	}

	private isCurrentTrackedTimeValid(isStrongValidation?: boolean): boolean {
		this.setDayInfo();
		if (isStrongValidation) {
			return this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual
				+ this.currentTimeEntry.timeValues.timeActual < MAX_TIMER_VALUE;
		} else {
			return this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual
				+ this.currentTimeEntry.timeValues.timeActual <= MAX_TIMER_VALUE;
		}
	}

	private isEstimatedTimeValid(): boolean {
		this.setDayInfo();
		return this.totalEstimatedTimeForDay - this.timeEntry.timeValues.timeEstimated
			+ this.currentTimeEntry.timeValues.timeEstimated < MAX_TIMER_VALUE;
	}

	private isFromToTimeValid(): boolean {
		return this.dayInfo.timeEntries
			.filter((timeEntry: TimeEntry) => timeEntry.timeOptions.isFromToShow && timeEntry.id !== this.currentTimeEntry.id)
			.every((timeEntry: TimeEntry) => {
				return timeEntry.timeValues.timeFrom >= this.currentTimeEntry.timeValues.timeTo
					|| this.currentTimeEntry.timeValues.timeFrom >= timeEntry.timeValues.timeTo;
			});
	}

	private isFromToTimeValid2(): boolean {
		return this.currentTimeEntry.timeValues.timeFrom > 0
			&& this.currentTimeEntry.timeValues.timeTo < MAX_TIMER_VALUE;
	}

	private isSubmitDataValid(): boolean {
		if (!this.isCurrentTrackedTimeValid()) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}
		if (!this.isEstimatedTimeValid()) {
			this.notificationService.danger('Total planned time should be less than 24 hours.');
			return false;
		}
		if (this.currentTimeEntry.timeOptions.isFromToShow && !this.isFromToTimeValid()) {
			this.notificationService.danger('Selected time period already exists.');
			return false;
		}
		if (this.currentTimeEntry.timeOptions.isFromToShow && !this.isFromToTimeValid2()) {
			this.notificationService.danger('Selected time period should be within one day.');
			return false;
		}

		return true;
	}

	ngOnDestroy() {
		if (this.timerSubscription) {
			this.timerSubscription.unsubscribe();
		}
	}

	private getFormHeight(): void {
		setTimeout(() => this.formHeight = this.el.nativeElement.offsetHeight, 0);
	}

	private loadProjects(): void {
		this.projectsService.getProjects(true).subscribe((res) => {
			this.projectList = this.removeNonActiveProjects(res);
			this.projectList = this.filterProjects(this.projectList);

			this.defaultProject = this.projectsService.defaultProject;
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
				this.loadTasks(this.currentTimeEntry.projectId);
			}

			if (this.projectModel) {
				this.timeEntry.projectId = this.projectModel.id;
				this.timeEntry.projectName = this.projectModel.name;
				this.timeEntry.color = this.projectModel.color;
			} else {
				this.loadTasks(this.currentTimeEntry.projectId);
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

	private setTimeActual(): void {
		this.currentTimeEntry.timeValues.timeFrom = this.convertFormValueToSeconds(this.timeFrom);
		this.currentTimeEntry.timeValues.timeTo = this.convertFormValueToSeconds(this.timeTo);
		this.currentTimeEntry.timeValues.timeActual = this.convertFormValueToSeconds(this.timeTo) -
			this.convertFormValueToSeconds(this.timeFrom);
	}

	private convertFormValueToSeconds(time: string): number {
		let arr = time.split(':');
		return (+arr[0] || 0) * 3600 + (+arr[1] || 0) * 60;
	}

	private convertTimeToString(time: number): string {
		let m = Math.floor(time / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;

		return ('00' + h).slice(-2) + ':' + ('00' + m).slice(-2);
	}

	private convertSecondsToTimeFormat(time: number): Time {
		let h = Math.floor(time / 3600 >> 0);
		let m = Math.floor(time / 60 >> 0) - h * 60;
		let s = time - m * 60 - h * 3600;
		return new Time(('00' + h).slice(-2), ('00' + m).slice(-2), ('00' + s).slice(-2));
	}

	private removeNonActiveProjects(projectList: Project[]): Project[] {
		let isUserAdmin = this.authService.isLoggedIn() && this.authService.authUser.role === Roles.admin;
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

	private setDayInfo(date?: string): void {
		this.dayInfo = this.calendarService.getDayInfoByDate(date || this.timeEntry.date);
		this.totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(this.dayInfo, 'timeActual');
		this.totalEstimatedTimeForDay = this.calendarService.getTotalTimeForDay(this.dayInfo, 'timeEstimated');
	}
}
