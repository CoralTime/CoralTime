import {
	Component, Input, OnInit, HostBinding, EventEmitter, Output, ElementRef
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Roles } from '../../../../core/auth/permissions';
import { TimeEntry, CalendarDay, Time } from '../../../../models/calendar';
import { Project } from '../../../../models/project';
import { Task } from '../../../../models/task';
import { User } from '../../../../models/user';
import { ArrayUtils } from '../../../../core/object-utils';
import { AuthService } from '../../../../core/auth/auth.service';
import { NotificationService } from '../../../../core/notification.service';
import { CalendarService } from '../../../../services/calendar.service';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { TasksService } from '../../../../services/tasks.service';
import { CalendarProjectsService } from '../../calendar-projects.service';
import { numberToHex } from '../../../../shared/form/color-picker/color-picker.component';
import { MAX_TIMER_VALUE } from '../../timer/timer.component';

@Component({
	selector: 'ct-entry-time-form',
	templateUrl: 'entry-time-form.component.html'
})

export class EntryTimeFormComponent implements OnInit {
	@HostBinding('class.ct-entry-time-form') addClass: boolean = true;

	@Input() timeEntry: TimeEntry;
	@Output() closeEntryTimeForm: EventEmitter<void> = new EventEmitter<void>();
	@Output() deleted: EventEmitter<void> = new EventEmitter<void>();

	currentTimeEntry: TimeEntry;
	formHeight: number;
	isActualTimeChanged: boolean;
	isEstimatedTimeShown: boolean;
	isEstimatedTimeChanged: boolean;
	isFromToFormChanged: boolean;
	isRequestLoading: boolean;
	isTasksLoading: boolean;
	projectList: Project[];
	projectModel: Project;
	taskList: Task[];
	taskModel: Task;
	timeActual: Time;
	timeEstimated: Time;
	timeFrom: Time = new Time('00', '00');
	timeTo: Time = new Time('00', '00');
	timeMask = [/\d/, /\d/];
	userInfo: User;

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
		this.isEstimatedTimeShown = this.timeEntry.timeValues.timeEstimated > 0;
		this.timeActual = this.convertSecondsToTimeFormat(this.timeEntry.timeValues.timeActual);
		this.timeEstimated = this.convertSecondsToTimeFormat(this.timeEntry.timeValues.timeEstimated);

		if (this.currentTimeEntry.timeOptions.isFromToShow) {
			this.fillFromToForm();
		}

		this.loadProjects();
		this.getFormHeight();
	}

	numberToHex(value: number): string {
		return numberToHex(value);
	}

	// PROJECT

	isArchivedProjectShown(): boolean {
		return this.currentTimeEntry.id && !this.currentTimeEntry.isProjectActive && !this.projectModel;
	}

	projectOnChange(projectModel: Project): void {
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
		this.currentTimeEntry.taskTypesId = taskModel.id;
		this.currentTimeEntry.taskName = taskModel.name;
	}

	// DESCRIPTION

	descriptionOnChange(): void {
		this.getFormHeight();
	}

	// FROM-TO FORM

	openFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = true;
	}

	closeFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = false;
		this.timeFrom = new Time('00', '00');
		this.timeTo = new Time('00', '00');
	}

	validateFromToForm(): void {
		this.isFromToFormChanged = true;
		this.timeTo = this.getMax(this.timeFrom, this.timeTo);
		this.setTimeActual();
		this.timeActual = this.convertSecondsToTimeFormat(this.currentTimeEntry.timeValues.timeActual);
	}

	private isFromToFormValueValid(): boolean {
		return this.convertTimeFormatToSeconds(this.timeTo) > this.convertTimeFormatToSeconds(this.timeFrom);
	}

	private fillFromToForm(): void {
		this.currentTimeEntry.timeOptions.isFromToShow = true;
		this.timeFrom = this.convertSecondsToTimeFormat(this.currentTimeEntry.timeValues.timeFrom);
		this.timeTo = this.convertSecondsToTimeFormat(this.currentTimeEntry.timeValues.timeTo);
	}

	private getMax(timeFrom: Time, timeTo: Time): Time {
		if (this.convertTimeFormatToSeconds(timeFrom) > this.convertTimeFormatToSeconds(timeTo)) {
			return new Time(timeFrom.hours, timeFrom.minutes);
		}

		return new Time(timeTo.hours, timeTo.minutes);
	}

	// TRACKING TIME

	timeActualOnChange(): void {
		this.closeFromToForm();
		this.isActualTimeChanged = true;
		this.currentTimeEntry.timeValues.timeActual = this.convertTimeFormatToSeconds(this.timeActual);
		this.currentTimeEntry.timeValues.timeFrom = null;
		this.currentTimeEntry.timeValues.timeTo = null;
	}

	timeEstimatedOnChange(): void {
		this.isEstimatedTimeChanged = true;
		this.currentTimeEntry.timeValues.timeEstimated = this.convertTimeFormatToSeconds(this.timeEstimated);
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
			() => {
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
		this.setDayInfo();
		return this.dayInfo.timeEntries
			.filter((timeEntry: TimeEntry) => timeEntry.timeOptions.isFromToShow && timeEntry.id !== this.currentTimeEntry.id)
			.every((timeEntry: TimeEntry) => {
				return timeEntry.timeValues.timeFrom >= this.currentTimeEntry.timeValues.timeTo
					|| this.currentTimeEntry.timeValues.timeFrom >= timeEntry.timeValues.timeTo;
			});
	}

	private isFromToTimeValid2(): boolean {
		return this.currentTimeEntry.timeValues.timeFrom >= 0
			&& this.currentTimeEntry.timeValues.timeTo < MAX_TIMER_VALUE;
	}

	private isSubmitDataValid(): boolean {
		if (this.isActualTimeChanged && !this.isCurrentTrackedTimeValid()) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}
		if (this.isEstimatedTimeChanged && !this.isEstimatedTimeValid()) {
			this.notificationService.danger('Total planned time should be less than 24 hours.');
			return false;
		}
		if (this.currentTimeEntry.timeOptions.isFromToShow && this.isFromToFormChanged && !this.isFromToTimeValid2()) {
			this.notificationService.danger('Selected time period should be within one day.');
			return false;
		}
		if (this.currentTimeEntry.timeOptions.isFromToShow && this.isFromToFormChanged && !this.isFromToTimeValid()) {
			this.notificationService.danger('Selected time period already exists.');
			return false;
		}

		return true;
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
		this.isTasksLoading = true;
		this.tasksService.getActiveTasks(projectId)
			.finally(() => this.isTasksLoading = false)
			.subscribe((res) => {
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
		this.currentTimeEntry.timeValues.timeFrom = this.convertTimeFormatToSeconds(this.timeFrom);
		this.currentTimeEntry.timeValues.timeTo = this.convertTimeFormatToSeconds(this.timeTo);
		this.currentTimeEntry.timeValues.timeActual = this.convertTimeFormatToSeconds(this.timeTo) -
			this.convertTimeFormatToSeconds(this.timeFrom);
	}

	private convertTimeFormatToSeconds(time: Time): number {
		return +time.hours * 3600 + +time.minutes * 60 + (+time.seconds || 0);
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
