import {
	Component, Input, ViewChild, EventEmitter, Output, OnInit, OnDestroy, QueryList,
	ViewChildren, ElementRef
} from '@angular/core';
import { TimeEntry, DateUtils } from '../../../../models/calendar';
import { Subscription, Observable } from 'rxjs';
import { CalendarService } from '../../../../services/calendar.service';
import { NotificationService } from '../../../../core/notification.service';
import { MultipleDatepickerComponent } from '../../entry-time/multiple-datepicker/multiple-datepicker.component';
import { MdDialogRef, MdDialog } from '@angular/material';
import { Time } from '../../entry-time/entry-time-form/entry-time-form.component';
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

	actualTime: Time;
	currentTimeEntry: TimeEntry;
	dialogRef: MdDialogRef<MultipleDatepickerComponent>;
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
	plannedTime: Time;
	selectedDate: Date;
	ticks: number;
	timerValue: string;
	timerSubscription: Subscription;

	private totalPlannedTimeForDay: number;
	private totalTrackedTimeForDay: number;

	constructor(private route: ActivatedRoute,
	            private calendarService: CalendarService,
	            private dialog: MdDialog,
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
		this.actualTime = this.splitTime(this.timeEntry.time);
		this.plannedTime = this.splitTime(this.timeEntry.plannedTime);
		this.selectedDate = this.currentTimeEntry.date;
		this.isUserManagerOnProject = this.timeEntry.isUserManagerOnProject;

		if (this.timeEntry.timeTimerStart > 0) {
			this.startTimer(DateUtils.getSecondsFromStartDay() - this.timeEntry.timeTimerStart);
		}

		this.checkTimeEntryStatus();
		this.setDayInfo();
	}

	calculateCalendarTaskHeight(): number {
		let taskHeight = Math.max(this.timeEntry.time / 3600, 1.5) * 95  - 42;
		return this.timeEntry.isFromToShow ? taskHeight - 25 : taskHeight;
	}

	duplicateAction(): void {
		this.dialogRef = this.dialog.open(MultipleDatepickerComponent, {
			width: '650px'
		});
		this.dialogRef.componentInstance.timeEntry = this.currentTimeEntry;
		this.dialogRef.componentInstance.actualTime = this.actualTime;
		this.dialogRef.componentInstance.plannedTime = this.plannedTime;
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

		this.currentTimeEntry.date = date[0] ? DateUtils.convertMomentToUTC(moment(date[0])) : this.currentTimeEntry.date;
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

	private isNewTrackedTimeValid(newDate: Date): boolean {
		this.setDayInfo(newDate);
		return this.totalTrackedTimeForDay + this.currentTimeEntry.time <= MAX_TIMER_VALUE;
	}

	private onSubmitDialog(dateList: Date[]): void {
		let observable: Observable<any>;

		if (dateList.some((date: Date) => !this.isNewTrackedTimeValid(date))) {
			this.notificationService.danger('Total actual time can\'t be more than 24 hours');
			return;
		}

		dateList.forEach((date: Date) => {
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

	private splitTime(time: number = 0): Time {
		let hours = Math.floor(time / 3600 >> 0);
		let minutes = Math.floor(time / 60 >> 0) - hours * 60;
		return new Time(this.formatTime(hours), this.formatTime(minutes));
	}

	private formatTime(time: number): string {
		return (time >= 0 && time < 10) ? '0' + time : time + '';
	}

	// ============

	setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	openEntryTimeForm() {
		if (this.isTimeEntryAvailable) {
			this.form.toggleEntryTimeForm();
		}
	}

	deleteTimeEntry() {
		this.timeEntryDeleted.emit();
	}

	checkTimer(): void {
		let obj: any;
		if (!DateUtils.isToday(new Date(this.timeEntry.date)) || !this.isTimeEntryAvailable) {
			obj = {
				time: MAX_TIMER_VALUE - (this.totalTrackedTimeForDay - this.timeEntry.time),
				timeFrom: this.totalTrackedTimeForDay - this.timeEntry.time,
				timeTo: MAX_TIMER_VALUE,
				timeTimerStart: -1
			};
		} else if (!this.isTrackedTimeValid()) {
			obj = {
				time: MAX_TIMER_VALUE - (this.totalTrackedTimeForDay - this.timeEntry.time),
				timeFrom: 0,
				timeTo: MAX_TIMER_VALUE - (this.totalTrackedTimeForDay - this.timeEntry.time),
				timeTimerStart: -1
			};
		}

		if (obj) {
			this.autoStopTimer();
			this.validateTimer(obj);
		}
	}

	updateTimer(): void {
		this.setDayInfo();
		this.isTimerShown ? this.autoStopTimer() : this.startTimer();
	}

	startTimer(timeTimerStart?: number): void {
		this.isTimerShown = true;
		timeTimerStart = timeTimerStart ? timeTimerStart + this.timeEntry.time : this.timeEntry.time;
		let timer = Observable.timer(0, 1000);

		this.timerSubscription = timer.subscribe(() => {
			this.ticks = timeTimerStart++;
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
			this.timerUpdated.emit();
			this.timerSubscription.unsubscribe();
			this.isTimerShown = false;
			this.calendarService.isTimerActivated = this.isTimerShown;
		});
	}

	ngOnDestroy() {
		if (this.timerSubscription) {
			this.autoStopTimer();
		}
	}

	private validateTimer(obj: any): Promise<any> {
		return this.calendarService.Patch(obj, this.timeEntry.id.toString())
			.toPromise().then(
				() => {
					this.calendarService.isTimerActivated = false;

					this.timeEntry.time = obj.time;
					this.timeEntry.timeFrom = obj.timeFrom;
					this.timeEntry.timeTo = obj.timeTo;
					this.timeEntry.timeTimerStart = obj.timeTimerStart;

					this.form.closeTimeEntryForm();
					this.notificationService.danger('Total actual time can\'t be more than 24 hours. Timer has stopped');
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status');
					return error;
				});
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
			this.isTimeEntryAvailable = !this.timeEntry.isLocked &&
				this.timeEntry.isProjectActive &&
				this.timeEntry.isTaskTypeActive;
		}
	}

	private setDayInfo(date?: Date): void {
		let dayInfo = this.calendarService.getDayInfoByDate(date || this.timeEntry.date);
		this.totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'time');
		this.totalPlannedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'plannedTime');
	}

	private isTrackedTimeValid(): boolean {
		return this.totalTrackedTimeForDay + this.ticks - this.timeEntry.time < MAX_TIMER_VALUE;
	}

	private saveTimerStatus(): Promise<any> {
		if (!this.isTimerShown) {
			this.currentTimeEntry.timeTimerStart = DateUtils.getSecondsFromStartDay();
			this.currentTimeEntry.isFromToShow = false;
		} else {
			this.currentTimeEntry.time = this.ticks;
			this.currentTimeEntry.timeFrom = DateUtils.getSecondsFromStartDay() - this.ticks;
			this.currentTimeEntry.timeTimerStart = -1;
			this.currentTimeEntry.timeTo = this.currentTimeEntry.timeFrom + this.ticks;
			this.actualTime = this.splitTime(this.currentTimeEntry.time);
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

	private saveTimeEntry(timeEntry: TimeEntry): void {
		for (let prop in this.timeEntry) {
			this.timeEntry[prop] = timeEntry[prop];
		}
	}

	// MENU DISPLAYING

	private isRightSideClear(el: HTMLElement): boolean {
		return window.innerWidth > el.getBoundingClientRect().right + 300;
	}

	private isLeftSideClear(el: HTMLElement): boolean {
		return el.getBoundingClientRect().left > 300;
	}
}
