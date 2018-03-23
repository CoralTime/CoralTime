import {
	Component, Input, ViewChild, EventEmitter, Output, OnInit, OnDestroy, QueryList,
	ViewChildren, ElementRef
} from '@angular/core';
import { TimeEntry, DateUtils, Time } from '../../../../models/calendar';
import { Subscription, Observable } from 'rxjs';
import { CalendarService } from '../../../../services/calendar.service';
import { NotificationService } from '../../../../core/notification.service';
import { MultipleDatepickerComponent } from '../../entry-time/multiple-datepicker/multiple-datepicker.component';
import { MatDialogRef, MatDialog } from '@angular/material';
import { MenuComponent } from '../../../../shared/menu/menu.component';
import { User } from '../../../../models/user';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { EntryTimeComponent } from '../../entry-time/entry-time.component';
import * as moment from 'moment';
import Moment = moment.Moment;

export const MAX_TIMER_VALUE = 86399;

@Component({
	selector: 'ct-calendar-task',
	templateUrl: 'calendar-task.component.html'
})

export class CalendarTaskComponent implements OnInit, OnDestroy {
	@Input() timeEntry: TimeEntry;
	@Output() timeEntryDeleted: EventEmitter<void> = new EventEmitter<void>();

	@Output() timerUpdated: EventEmitter<void> = new EventEmitter<void>();
	@Output() closeEntryTimeForm: EventEmitter<void> = new EventEmitter<void>();
	@ViewChild('form') form: EntryTimeComponent;
	@ViewChildren(MenuComponent) menuList: QueryList<MenuComponent>;

	currentTimeEntry: TimeEntry;
	dialogRef: MatDialogRef<MultipleDatepickerComponent>;
	isCalendarShown: boolean = false;
	isOpenLeft: boolean = false;
	isOpenRight: boolean = false;
	isOpenMobile: boolean = false;
	isTimeEntryAvailable: boolean;
	isTimerShown: boolean = false;
	isUserAdmin: boolean;
	isUserManagerOnProject: boolean;
	firstDayOfWeek: number;
	lockReason: string = '';
	selectedDate: string;
	ticks: number;
	timeActual: Time;
	timeEstimated: Time;
	timerValue: string;
	timerSubscription: Subscription;

	private totalEstimatedTimeForDay: number;
	private totalTrackedTimeForDay: number;

	constructor(private route: ActivatedRoute,
	            private calendarService: CalendarService,
	            private dialog: MatDialog,
	            private elementRef: ElementRef,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			let user = this.impersonationService.impersonationUser || data.user;
			this.firstDayOfWeek = user.weekStart;
			this.isUserAdmin = data.user.isAdmin;
		});
		this.currentTimeEntry = new TimeEntry(this.timeEntry);
		this.timeActual = this.splitTime(this.timeEntry.timeValues.timeActual);
		this.timeEstimated = this.splitTime(this.timeEntry.timeValues.timeEstimated);
		this.selectedDate = this.currentTimeEntry.date;
		this.isUserManagerOnProject = this.timeEntry.isUserManagerOnProject;

		if (this.timeEntry.timeOptions.timeTimerStart && this.timeEntry.timeOptions.timeTimerStart !== -1) {
			this.startTimer();
		}

		this.checkTimeEntryStatus();
		this.setDayInfo();
	}

	// MENU ACTIONS

	deleteTimeEntry() {
		this.timeEntryDeleted.emit();
	}

	duplicateAction(): void {
		this.dialogRef = this.dialog.open(MultipleDatepickerComponent, {
			width: '650px'
		});
		this.dialogRef.componentInstance.timeEntry = this.currentTimeEntry;
		this.dialogRef.componentInstance.timeActual = this.timeActual;
		this.dialogRef.componentInstance.timeEstimated = this.timeEstimated;
		this.dialogRef.componentInstance.firstDayOfWeek = this.firstDayOfWeek;

		this.dialogRef.componentInstance.onSubmit.subscribe((event) => {
			this.onSubmitDialog(event);
		});
	}

	moveAction(trigger: MenuComponent): void {
		this.isCalendarShown = true;
		this.isOpenRight = this.isRightSideClear(this.elementRef.nativeElement);
		this.isOpenLeft = !this.isOpenRight && this.isLeftSideClear(this.elementRef.nativeElement);
		this.isOpenMobile = !this.isOpenRight && !this.isOpenLeft;
		trigger.toggleMenu();
	}

	deleteAction(): void {
		let observable: Observable<any>;
		observable = this.calendarService.Delete(this.currentTimeEntry.id.toString());

		observable.subscribe(
			() => {
				this.notificationService.success('Time Entry has been deleted.');
				this.calendarService.timeEntriesUpdated.emit();
				this.closeForm();
			},
			error => {
				this.notificationService.danger('Error deleting Time Entry');
			});
	}

	closeForm(): void {
		this.closeEntryTimeForm.emit();
	}

	closeAllMenus(): void {
		this.isCalendarShown = false;
		this.menuList.forEach((menu) => menu.closeMenu());
	}

	dateOnChange(date: Moment[] | string[]): void {
		if (date instanceof moment) {
			return;
		}

		this.currentTimeEntry.date = date[0] ? DateUtils.formatDateToString(date[0]) : this.currentTimeEntry.date;
		if (!this.isNewTrackedTimeValid(this.currentTimeEntry.date)) {
			this.notificationService.danger('Total actual time can\'t be more than 24 hours');
			this.closeAllMenus();
			return;
		}

		let observable = this.calendarService.Put(this.currentTimeEntry, this.currentTimeEntry.id.toString());

		observable.subscribe(
			() => {
				this.notificationService.success('New Time Entry has been successfully moved.');
				this.calendarService.timeEntriesUpdated.emit();
				this.closeEntryTimeForm.emit();
			},
			error => {
				this.notificationService.danger('Error moving Time Entry');
			});
		this.closeAllMenus();
	}

	private isNewTrackedTimeValid(newDate: string): boolean {
		this.setDayInfo(newDate);
		return this.totalTrackedTimeForDay + this.currentTimeEntry.timeValues.timeActual <= MAX_TIMER_VALUE;
	}

	private onSubmitDialog(dateList: string[]): void {
		let observable: Observable<any>;

		if (dateList.some((date: string) => !this.isNewTrackedTimeValid(date))) {
			this.notificationService.danger('Total actual time can\'t be more than 24 hours');
			return;
		}

		dateList.forEach((date: string) => {
			this.currentTimeEntry.date = date;
			observable = this.calendarService.Post(this.currentTimeEntry);

			observable.subscribe(
				() => {
					this.notificationService.success('New Time Entry has been successfully dublicated.');
					this.calendarService.timeEntriesUpdated.emit();
				},
				error => {
					this.notificationService.danger('Error dublicating Time Entry');
				});
		});
	}

	// TIMER ACTIONS

	checkTimer(): void {
		let isChanged: boolean = false;

		if (!DateUtils.isToday(this.timeEntry.date) || !this.isTimeEntryAvailable) {
			isChanged = true;
			this.currentTimeEntry.timeOptions = {
				isFromToShow: true,
				timeTimerStart: -1
			};
			this.currentTimeEntry.timeValues = {
				timeActual: this.ticks,
				timeEstimated: this.currentTimeEntry.timeValues.timeEstimated,
				timeFrom: MAX_TIMER_VALUE - this.ticks,
				timeTo: MAX_TIMER_VALUE
			};
		} else if (!this.isTrackedTimeValid()) {
			isChanged = true;
			this.currentTimeEntry.timeOptions = {
				isFromToShow: true,
				timeTimerStart: -1
			};
			this.currentTimeEntry.timeValues = {
				timeActual: MAX_TIMER_VALUE - (this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual),
				timeEstimated: this.currentTimeEntry.timeValues.timeEstimated,
				timeFrom: 0,
				timeTo: MAX_TIMER_VALUE - (this.totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual)
			};
		}

		if (isChanged) {
			this.autoStopTimer();
			this.changeTimerStatus(this.currentTimeEntry);
		}
	}

	updateTimer(): void {
		this.setDayInfo();
		this.isTimerShown ? this.autoStopTimer() : this.startTimer();
	}

	startTimer(): void {
		this.isTimerShown = true;
		let timer = Observable.timer(0, 1000);

		this.timerSubscription = timer.subscribe(() => {
			this.ticks = DateUtils.getSecondsFromStartDay(true) - this.timeEntry.timeOptions.timeTimerStart
				+ this.timeEntry.timeValues.timeActual;
			this.timerValue = this.setTimeString(this.ticks);
			this.checkTimer();
		});
	}

	autoStopTimer(): void {
		if (this.timerSubscription) {
			this.timerSubscription.unsubscribe();
		}

		this.isTimerShown = false;
	}

	stopTimer(): void {
		this.calendarService.isTimerActivated = false;
		this.saveTimerStatus().then((err: any) => {
			if (err) {
				return;
			}

			this.isTimerShown = false;
			this.timerUpdated.emit();
			this.timerSubscription.unsubscribe();
			this.calendarService.isTimerActivated = this.isTimerShown;
		});
	}

	private changeTimerStatus(timeEntry: TimeEntry): void {
		this.calendarService.Put(timeEntry, this.timeEntry.id.toString())
			.toPromise().then(
			() => {
				this.calendarService.isTimerActivated = false;

				this.timeEntry.timeValues = timeEntry.timeValues;
				this.timeEntry.timeOptions = timeEntry.timeOptions;

				this.form.closeTimeEntryForm();
				this.notificationService.danger('Total actual time can\'t be more than 24 hours. Timer has stopped');
				return null;
			},
			error => {
				this.notificationService.danger('Error changing Timer status');
				return error;
			});
	}

	private isTrackedTimeValid(): boolean {
		return this.totalTrackedTimeForDay + this.ticks - this.timeEntry.timeValues.timeActual < MAX_TIMER_VALUE;
	}

	private saveTimerStatus(): Promise<any> {
		if (!this.isTimerShown) {
			this.currentTimeEntry.timeOptions = {
				isFromToShow: false,
				timeTimerStart: DateUtils.getSecondsFromStartDay(true)
			};
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
			this.timeActual = this.splitTime(this.currentTimeEntry.timeValues.timeActual);
		}

		return this.calendarService.Put(this.currentTimeEntry, this.currentTimeEntry.id.toString())
			.toPromise().then(
				() => {
					this.saveTimeEntry(this.currentTimeEntry);
					this.notificationService.success('Timer has stopped.');
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status');
					return error;
				});
	}

	// GENERAL

	calculateCalendarTaskHeight(): number {
		let taskHeight = Math.max(this.timeEntry.timeValues.timeActual / 3600, 1.5) * 95  - 42;
		return this.timeEntry.timeOptions.isFromToShow ? taskHeight - 25 : taskHeight;
	}

	openEntryTimeForm() {
		if (this.isTimeEntryAvailable) {
			this.form.toggleEntryTimeForm();
		}
	}

	setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	ngOnDestroy() {
		if (this.timerSubscription) {
			this.autoStopTimer();
		}
	}

	private checkTimeEntryStatus(): void {
		if (this.timeEntry.isLocked) {
			this.lockReason += '\nTime Entry is locked, because the selected date is in the lock time entry period for the project.';
		}

		if (!this.timeEntry.isProjectActive) {
			this.lockReason += '\nTime Entry is locked, because the project is archived.';
		}

		if (!this.timeEntry.isTaskTypeActive) {
			this.lockReason += '\nTime Entry is locked, because the task is archived.';
		}

		if (this.isUserAdmin || this.isUserManagerOnProject) {
			this.isTimeEntryAvailable = true;
		} else {
			this.isTimeEntryAvailable = !this.timeEntry.isLocked
				&& this.timeEntry.isProjectActive && this.timeEntry.isTaskTypeActive;
		}
	}

	private formatTime(time: number): string {
		return (time >= 0 && time < 10) ? '0' + time : time + '';
	}

	private setDayInfo(date?: string): void {
		let dayInfo = this.calendarService.getDayInfoByDate(date || this.timeEntry.date);
		this.totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'timeActual');
		this.totalEstimatedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'timeEstimated');
	}

	private saveTimeEntry(timeEntry: TimeEntry): void {
		for (let prop in this.timeEntry) {
			this.timeEntry[prop] = timeEntry[prop];
		}
	}

	private splitTime(time: number = 0): Time {
		let hours = Math.floor(time / 3600 >> 0);
		let minutes = Math.floor(time / 60 >> 0) - hours * 60;
		return new Time(this.formatTime(hours), this.formatTime(minutes));
	}

	// MENU DISPLAYING

	private isRightSideClear(el: HTMLElement): boolean {
		return window.innerWidth > el.getBoundingClientRect().right + 300;
	}

	private isLeftSideClear(el: HTMLElement): boolean {
		return el.getBoundingClientRect().left > 300;
	}
}
