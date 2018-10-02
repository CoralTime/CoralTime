import {
	Component, Input, ViewChild, EventEmitter, Output, OnInit, OnDestroy, QueryList,
	ViewChildren, ElementRef
} from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import Moment = moment.Moment;
import { Subscription, Observable } from 'rxjs';
import { TimeEntry, DateUtils, CalendarDay } from '../../../../models/calendar';
import { User } from '../../../../models/user';
import { NotificationService } from '../../../../core/notification.service';
import { CalendarService } from '../../../../services/calendar.service';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { EntryTimeComponent } from '../../entry-time/entry-time.component';
import { MultipleDatepickerComponent } from '../../entry-time/multiple-datepicker/multiple-datepicker.component';
import { numberToHex } from '../../../../shared/form/color-picker/color-picker.component';
import { MenuComponent } from '../../../../shared/menu/menu.component';

export const MAX_TIMER_VALUE = 86399;

@Component({
	selector: 'ct-calendar-task',
	templateUrl: 'calendar-task.component.html'
})

export class CalendarTaskComponent implements OnInit, OnDestroy {
	@Input() timeEntry: TimeEntry;

	@Output() closeEntryTimeForm: EventEmitter<void> = new EventEmitter<void>();
	@Output() timeEntryDeleted: EventEmitter<void> = new EventEmitter<void>();
	@Output() timerUpdated: EventEmitter<void> = new EventEmitter<void>();

	@ViewChild('form') form: EntryTimeComponent;
	@ViewChildren(MenuComponent) menuList: QueryList<MenuComponent>;

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
	timeFormat: number;
	timerValue: string;
	timerSubscription: Subscription;

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
			this.timeFormat = data.user.timeFormat;
		});

		this.selectedDate = this.timeEntry.date;
		this.isUserManagerOnProject = this.timeEntry.isUserManagerOnProject;

		// if (this.timeEntry.timeOptions.timeTimerStart && this.timeEntry.timeOptions.timeTimerStart !== -1) {
		// 	this.startTimer();
		// }

		this.checkTimeEntryStatus();
		this.getDayInfo();
	}

	// MENU ACTIONS

	deleteTimeEntry() {
		this.timeEntryDeleted.emit();
	}

	duplicateAction(): void {
		this.dialogRef = this.dialog.open(MultipleDatepickerComponent, {
			width: '650px'
		});

		this.dialogRef.componentInstance.firstDayOfWeek = this.firstDayOfWeek;
		this.dialogRef.componentInstance.timeEntry = this.timeEntry;

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
		observable = this.calendarService.Delete(this.timeEntry.id.toString());

		observable.subscribe(
			() => {
				this.notificationService.success('Time Entry has been deleted.');
				this.calendarService.timeEntriesUpdated.emit();
				this.closeForm();
			},
			() => {
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

		let currentTimeEntry = new TimeEntry(this.timeEntry);
		currentTimeEntry.date = date[0] ? DateUtils.formatDateToString(date[0]) : currentTimeEntry.date;

		if (!this.isSubmitDataValid(currentTimeEntry.date)) {
			this.closeAllMenus();
			return;
		}

		let observable = this.calendarService.Put(currentTimeEntry, currentTimeEntry.id.toString());

		observable.subscribe(
			() => {
				this.notificationService.success('New Time Entry has been successfully moved.');
				this.calendarService.timeEntriesUpdated.emit();
				this.closeEntryTimeForm.emit();
			},
			() => {
				this.notificationService.danger('Error moving Time Entry.');
			});
		this.closeAllMenus();
	}

	onSubmitDialog(dateList: string[]): void {
		let observableList: Observable<any>[] = [];

		if (dateList.some((date: string) => !this.isSubmitDataValid(date))) {
			return;
		}

		dateList.forEach((date: string) => {
			let currentTimeEntry = new TimeEntry(this.timeEntry);
			currentTimeEntry.date = date;
			observableList.push(this.calendarService.Post(currentTimeEntry));
		});

		Observable.forkJoin(observableList).subscribe(
			() => {
				this.notificationService.success('New Time Entry has been successfully dublicated.');
				this.calendarService.timeEntriesUpdated.emit();
			},
			() => {
				this.notificationService.danger('Error dublicating Time Entry.');
			});
	}

	private isNewTrackedTimeValid(newDate: string): boolean {
		let totalTrackedTimeForDay = this.getTotalTime(this.getDayInfo(newDate), 'timeActual');
		return totalTrackedTimeForDay + this.timeEntry.timeValues.timeActual <= MAX_TIMER_VALUE;
	}

	private isNewPlannedTimeValid(newDate: string): boolean {
		let totalEstimatedTimeForDay = this.getTotalTime(this.getDayInfo(newDate), 'timeEstimated');
		return totalEstimatedTimeForDay + this.timeEntry.timeValues.timeEstimated <= MAX_TIMER_VALUE;
	}

	private isFromToTimeValid(newDate: string): boolean {
		let dayInfo = this.getDayInfo(newDate);
		return !dayInfo || dayInfo.timeEntries
			.filter((timeEntry: TimeEntry) => timeEntry.timeOptions.isFromToShow && timeEntry.id !== this.timeEntry.id)
			.every((timeEntry: TimeEntry) => {
				return timeEntry.timeValues.timeFrom >= this.timeEntry.timeValues.timeTo
					|| this.timeEntry.timeValues.timeFrom >= timeEntry.timeValues.timeTo;
			});
	}

	private isSubmitDataValid(date: string): boolean {
		if (!this.isNewTrackedTimeValid(date)) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}

		if (!this.isNewPlannedTimeValid(date)) {
			this.notificationService.danger('Total planned time should be less than 24 hours.');
			return false;
		}

		if (!this.isFromToTimeValid(date)) {
			this.notificationService.danger('Selected time period already exists.');
			return false;
		}

		return true;
	}

	// TIMER ACTIONS

	checkTimer(): void {
		let errorMessage: string;

		if (!DateUtils.isToday(this.timeEntry.date)) {
			errorMessage = 'Selected time period should be within one day. Timer has stopped.'
		}
		if (!errorMessage && !this.isTimeEntryAvailable) {
			errorMessage = 'Timer has stopped, because Time Entry is locked.';
		}
		if (!errorMessage && !this.isTrackedTimeValid()) {
			errorMessage = 'Total actual time should be less than 24 hours. Timer has stopped.';
		}

		if (errorMessage) {
			let currentTimeEntry = new TimeEntry(this.timeEntry);
			let totalTrackedTimeForDay = this.getTotalTime(this.getDayInfo(), 'timeActual');
			let finalTimerValue = MAX_TIMER_VALUE - (totalTrackedTimeForDay - this.timeEntry.timeValues.timeActual);
			let timeTimerStart = this.limitTimerValue(this.timeEntry.timeOptions.timeTimerStart - this.timeEntry.timeValues.timeActual
				- new Date().getTimezoneOffset() * 60); // locale format

			currentTimeEntry.timeOptions = {
				isFromToShow: true,
				timeTimerStart: -1
			};
			currentTimeEntry.timeValues = {
				timeActual: Math.min(MAX_TIMER_VALUE - timeTimerStart, finalTimerValue),
				timeEstimated: this.timeEntry.timeValues.timeEstimated,
				timeFrom: timeTimerStart,
				timeTo: Math.min(MAX_TIMER_VALUE - timeTimerStart, finalTimerValue) + timeTimerStart
			};

			this.autoStopTimer();
			this.changeTimerStatus(currentTimeEntry, errorMessage);
		}
	}

	updateTimer(): void {
		this.getDayInfo();
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
		// this.calendarService.isTimerActivated = false;
		this.saveTimerStatus().then((err: any) => {
			if (err) {
				return;
			}

			this.isTimerShown = false;
			this.timerUpdated.emit();
			this.timerSubscription.unsubscribe();
			// this.calendarService.isTimerActivated = this.isTimerShown;
		});
	}

	private changeTimerStatus(timeEntry: TimeEntry, errorMessage: string): void {
		this.calendarService.Put(timeEntry, this.timeEntry.id.toString())
			.toPromise().then(
			() => {
				// this.calendarService.isTimerActivated = false;
				this.timeEntry.timeValues = timeEntry.timeValues;
				this.timeEntry.timeOptions = timeEntry.timeOptions;
				this.form.closeTimeEntryForm();

				this.notificationService.danger(errorMessage);
				return null;
			},
			error => {
				this.notificationService.danger('Error changing Timer status.');
				return error;
			});
	}

	private isTrackedTimeValid(): boolean {
		let totalTrackedTimeForDay = this.getTotalTime(this.getDayInfo(), 'timeActual');
		return totalTrackedTimeForDay + this.ticks - this.timeEntry.timeValues.timeActual < MAX_TIMER_VALUE;
	}

	private limitTimerValue(time: number): number {
		return Math.min(Math.max(time, 0), MAX_TIMER_VALUE);
	}

	private saveTimerStatus(): Promise<any> {
		let currentTimeEntry = new TimeEntry(this.timeEntry);

		if (!this.isTimerShown) {
			currentTimeEntry.timeOptions = {
				isFromToShow: false,
				timeTimerStart: DateUtils.getSecondsFromStartDay(true)
			};
		} else {
			currentTimeEntry.timeOptions = {
				isFromToShow: true,
				timeTimerStart: -1
			};

			let secondsFromStartDay = this.roundTime(DateUtils.getSecondsFromStartDay(false));
			let roundTicks = (this.ticks < 60) ? 60 : this.roundTime(this.ticks);

			currentTimeEntry.timeValues = {
				timeActual: roundTicks,
				timeEstimated: this.timeEntry.timeValues.timeEstimated,
				timeFrom: Math.max(secondsFromStartDay - roundTicks, 0),
				timeTo: Math.max(secondsFromStartDay - roundTicks, 0) + roundTicks
			};
		}

		return this.calendarService.Put(currentTimeEntry, currentTimeEntry.id.toString())
			.toPromise().then(
				() => {
					this.saveTimeEntry(currentTimeEntry);
					this.notificationService.success('Timer has stopped.');
					return null;
				},
				error => {
					this.notificationService.danger('Error changing Timer status.');
					return error;
				});
	}

	private roundTime(time: number): number {
		return time - time % 60;
	}

	// GENERAL

	calculateCalendarTaskHeight(): number {
		let taskHeight = Math.max(this.timeEntry.timeValues.timeActual / 3600, 1.5) * 125 - 96;
		return this.timeEntry.timeOptions.isFromToShow ? taskHeight - 25 : taskHeight;
	}

	numberToHex(value: number): string {
		return numberToHex(value);
	}

	openEntryTimeForm() {
		if (this.isTimeEntryAvailable) {
			this.form.toggleEntryTimeForm();
		}
	}

	setTimeString(s: number, formatToAmPm: boolean = false): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;

		if (formatToAmPm) {
			let t = new Date().setHours(0, 0, s);
			return moment(t).format('hh:mm A');
		}

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

	private getDayInfo(date?: string): CalendarDay {
		return this.calendarService.getDayInfoByDate(date || this.timeEntry.date);
	}

	private getTotalTime(dayInfo: CalendarDay, field: string): number {
		return this.calendarService.getTotalTimeForDay(dayInfo, field);
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
