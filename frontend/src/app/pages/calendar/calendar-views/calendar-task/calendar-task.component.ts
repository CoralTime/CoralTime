import {
	Component, Input, ViewChild, EventEmitter, Output, OnInit, QueryList, ViewChildren, ElementRef
} from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import Moment = moment.Moment;
import { Observable } from 'rxjs';
import { TimeEntry, DateUtils, CalendarDay } from '../../../../models/calendar';
import { User } from '../../../../models/user';
import { NotificationService } from '../../../../core/notification.service';
import { CalendarService } from '../../../../services/calendar.service';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { EntryTimeComponent } from '../../entry-time/entry-time.component';
import { MultipleDatepickerComponent } from '../../entry-time/multiple-datepicker/multiple-datepicker.component';
import { numberToHex } from '../../../../shared/form/color-picker/color-picker.component';
import { MenuComponent } from '../../../../shared/menu/menu.component';
import { MAX_TIMER_VALUE } from '../../timer/timer.component';

@Component({
	selector: 'ct-calendar-task',
	templateUrl: 'calendar-task.component.html'
})

export class CalendarTaskComponent implements OnInit {
	@Input() timeEntry: TimeEntry;

	@Output() closeEntryTimeForm: EventEmitter<void> = new EventEmitter<void>();
	@Output() timeEntryDeleted: EventEmitter<void> = new EventEmitter<void>();

	@ViewChild('form') form: EntryTimeComponent;
	@ViewChildren(MenuComponent) menuList: QueryList<MenuComponent>;

	dialogRef: MatDialogRef<MultipleDatepickerComponent>;
	isCalendarShown: boolean = false;
	isOpenLeft: boolean = false;
	isOpenRight: boolean = false;
	isOpenMobile: boolean = false;
	isTimeEntryAvailable: boolean;
	isUserAdmin: boolean;
	isUserManagerOnProject: boolean;
	firstDayOfWeek: number;
	lockReason: string = '';
	selectedDate: string;
	timeFormat: number;

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

	// GENERAL

	calculateCalendarTaskHeight(): number {
		const fromToHeight = 25;
		const gap = 6; // vertical distance between timeEntries
		const staticHeight = 66 + gap;
		const heightOfOneHour = 65; // 520/8 // 520px - available place for timeEntries
		const minHours = 2.5; // (staticHeight + 93)/heightOfOneHour // 93px - minHeight for description with 1 line
		const taskHeight = Math.max(this.timeEntry.timeValues.timeActual / 3600, minHours) * heightOfOneHour - staticHeight;
		return this.timeEntry.timeOptions.isFromToShow ? taskHeight - fromToHeight : taskHeight;
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

	// MENU DISPLAYING

	private isRightSideClear(el: HTMLElement): boolean {
		return window.innerWidth > el.getBoundingClientRect().right + 300;
	}

	private isLeftSideClear(el: HTMLElement): boolean {
		return el.getBoundingClientRect().left > 300;
	}
}
