import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { Observable } from 'rxjs/Observable';
import { CalendarDay, DateUtils, TimeEntry } from '../../../../models/calendar';
import { User } from '../../../../models/user';
import { NotificationService } from '../../../../core/notification.service';
import { CalendarService } from '../../../../services/calendar.service';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { ctCalendarDayAnimation } from '../../calendar.animation';
import { EntryTimeComponent } from '../../entry-time/entry-time.component';
import { MAX_TIMER_VALUE } from '../../timer/timer.component';

@Component({
	selector: 'ct-calendar-day',
	templateUrl: 'calendar-day.component.html',
	animations: [ctCalendarDayAnimation.slideCalendarTask]
})

export class CalendarDayComponent implements OnInit {
	@Input() animationDelay: number;
	@Input() dayInfo: CalendarDay;
	@ViewChild('entryForm') entryForm: EntryTimeComponent;

	animationState: string;
	canChangeDragEnter: boolean = true;
	changeDragEnterTimeout: any;
	draggedTimeEntry: TimeEntry;
	isEntryFormOpened: boolean = false;
	isDragEnter: boolean;
	fakeCalendarTaskHeight: number;
	newTimeEntry: TimeEntry;
	user: User;

	@ViewChild('calendarTask', {read: ElementRef}) calendarTask: ElementRef;

	constructor(private calendarService: CalendarService,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private route: ActivatedRoute) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			this.user = this.impersonationService.impersonationUser || data.user;
		});

		this.triggerAnimation();
	}

	addNewTimeEntry(currentDate: string): void {
		let newTimeEntry = {
			date: currentDate,
			projectName: 'Select Project',
			taskName: 'Select Task'
		};
		this.newTimeEntry = new TimeEntry(newTimeEntry);
		this.isEntryFormOpened = true;
		setTimeout(() => {
			this.entryForm.toggleEntryTimeForm();
		}, 0);
	}

	calcTime(type: string): string {
		return this.setTimeString(this.getGeneralTime(type));
	}

	getGeneralTime(type: string): number {
		let timeEnries: TimeEntry[] = this.dayInfo.timeEntries;
		let time: number = 0;
		if (timeEnries) {
			for (let i = 0; i < timeEnries.length; i++) {
				time += timeEnries[i].timeValues[type];
			}
		}

		return time;
	}

	private setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	deleteTimeEntry(index?: number): void {
		this.isEntryFormOpened = false;
	}

	triggerAnimation(): void {
		this.animationState = 'hide';
		setTimeout(() => this.animationState = 'show', this.animationDelay);
	}

	// DRAG ACTIONS

	dragStart(timeEntry: TimeEntry, target: HTMLElement): void {
		if (this.isTimeEntryFormOpened()) {
			return;
		}

		let calendarTaskHeight = target.clientHeight;
		target.classList.add('ct-dragged-task');
		this.calendarService.fakeCalendarTaskHeight = calendarTaskHeight;
		this.calendarService.draggedTimeEntry = timeEntry;
	}

	dragEnd(target: HTMLElement): void {
		target.classList.remove('ct-dragged-task');
		this.calendarService.draggedTimeEntry = null;
		this.calendarService.fakeCalendarTaskHeight = null;
	}

	drop(): void {
		let submitObservable: Observable<any>;
		if (this.draggedTimeEntry) {
			if (DateUtils.formatDateToString(this.draggedTimeEntry.date) === this.dayInfo.date) {
				return;
			}
			if (!this.isSubmitDataValid(this.dayInfo.date, this.draggedTimeEntry)) {
				return;
			}
			this.draggedTimeEntry.date = this.dayInfo.date;
			this.draggedTimeEntry.timeOptions.timeTimerStart = -1;

			if (this.isAltPressed()) {
				submitObservable = this.calendarService.Post(this.draggedTimeEntry);
			} else {
				submitObservable = this.calendarService.Put(this.draggedTimeEntry, this.draggedTimeEntry.id.toString());
			}

			submitObservable.subscribe(
				() => {
					if (this.isAltPressed()) {
						this.notificationService.success('New Time Entry has been successfully created.');
					} else {
						this.notificationService.success('New Time Entry has been successfully moved.');
					}
					this.calendarService.timeEntriesUpdated.emit();
					this.draggedTimeEntry = null;
				},
				() => {
					this.notificationService.danger('Error moving Time Entry.');
				});
		}
	}

	onDragLeave(): void {
		if (this.isDragEnter && this.canChangeDragEnter) {
			this.isDragEnter = false;
			this.fakeCalendarTaskHeight = 0;
			this.draggedTimeEntry = null;
		}
		this.changeDragEnter();
	}

	onDragOver(): void {
		if (!this.isDragEnter && this.canChangeDragEnter) {
			this.fakeCalendarTaskHeight = this.calendarService.fakeCalendarTaskHeight;
			this.draggedTimeEntry = this.calendarService.draggedTimeEntry;
			this.isDragEnter = true;
		}
		this.changeDragEnter();
	}

	dragEffect(): string {
		return this.calendarService.dragEffect;
	}

	// GENERAL

	getDateNumber(date: string): number {
		return DateUtils.formatStringToDate(date).getDate();
	}

	getDateString(date: string): string {
		return moment(date).format('dddd');
	}

	isAltPressed(): boolean {
		return this.calendarService.isAltPressed;
	}

	isToday(date: string): boolean {
		return DateUtils.isToday(date);
	}

	isTimeEntryFormOpened(): boolean {
		return this.calendarService.isTimeEntryFormOpened;
	}

	private changeDragEnter(): void {
		clearTimeout(this.changeDragEnterTimeout);
		this.canChangeDragEnter = false;
		this.changeDragEnterTimeout = setTimeout(() => {
			this.canChangeDragEnter = true;
			this.isDragEnter = false;
		}, 200);
	}

	private getDayInfo(date: string): CalendarDay {
		return this.calendarService.getDayInfoByDate(date);
	}

	private getTotalTime(dayInfo: CalendarDay, field: string): number {
		return this.calendarService.getTotalTimeForDay(dayInfo, field);
	}

	private isNewTrackedTimeValid(newDate: string, timeEntry: TimeEntry): boolean {
		let totalActualTimeForDay = this.getTotalTime(this.getDayInfo(newDate), 'timeActual');
		return totalActualTimeForDay + timeEntry.timeValues.timeActual <= MAX_TIMER_VALUE;
	}

	private isNewPlannedTimeValid(newDate: string, timeEntry: TimeEntry): boolean {
		let totalEstimatedTimeForDay = this.getTotalTime(this.getDayInfo(newDate), 'timeEstimated');
		return totalEstimatedTimeForDay + timeEntry.timeValues.timeEstimated <= MAX_TIMER_VALUE;
	}

	private isFromToTimeValid(newDate: string, timeEntry: TimeEntry): boolean {
		let dayInfo = this.getDayInfo(newDate);
		return !dayInfo || dayInfo.timeEntries
			.filter((item: TimeEntry) => item.timeOptions.isFromToShow && item.id !== timeEntry.id)
			.every((item: TimeEntry) => {
				return item.timeValues.timeFrom >= timeEntry.timeValues.timeTo
					|| timeEntry.timeValues.timeFrom >= item.timeValues.timeTo;
			});
	}

	private isSubmitDataValid(date: string, timeEntry: TimeEntry): boolean {
		if (!this.isNewTrackedTimeValid(date, timeEntry)) {
			this.notificationService.danger('Total actual time should be less than 24 hours.');
			return false;
		}

		if (!this.isNewPlannedTimeValid(date, timeEntry)) {
			this.notificationService.danger('Total planned time should be less than 24 hours.');
			return false;
		}

		if (!this.isFromToTimeValid(date, timeEntry)) {
			this.notificationService.danger('Selected time period already exists.');
			return false;
		}

		return true;
	}
}
