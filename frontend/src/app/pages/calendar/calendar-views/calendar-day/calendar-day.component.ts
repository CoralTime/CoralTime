import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { CalendarDay, DateUtils, TimeEntry } from '../../../../models/calendar';
import { CalendarService } from '../../../../services/calendar.service';
import { EntryTimeComponent } from '../../entry-time/entry-time.component';
import { NotificationService } from '../../../../core/notification.service';
import { MAX_TIMER_VALUE } from '../calendar-task/calendar-task.component';
import { Observable } from 'rxjs/Observable';
import * as moment from 'moment';

@Component({
	selector: 'ct-calendar-day',
	templateUrl: 'calendar-day.component.html'
})

export class CalendarDayComponent implements OnInit {
	@Input() dayInfo: CalendarDay;
	@ViewChild('entryForm') entryForm: EntryTimeComponent;

	canChangeDragEnter: boolean = true;
	changeDragEnterTimeout: any;
	draggedTimeEntry: TimeEntry;
	isEntryFormOpened: boolean = false;
	isDragEnter: boolean;
	fakeCalendarTaskHeight: number;
	newTimeEntry: TimeEntry;

	@ViewChild('calendarTask', {read: ElementRef}) calendarTask: ElementRef;

	constructor(private calendarService: CalendarService,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		if (this.isToday(this.dayInfo.date)) {
			this.calendarService.isTimerActivated = !!this.dayInfo.timeEntries.find((timeEntry) =>
				timeEntry.timeOptions.timeTimerStart && timeEntry.timeOptions.timeTimerStart !== -1);
		}
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
		let timeEnries: TimeEntry[] = this.dayInfo.timeEntries;
		let time: number = 0;
		if (timeEnries) {
			for (let i = 0; i < timeEnries.length; i++) {
				time += timeEnries[i].timeValues[type];
			}
		}

		return this.setTimeString(time);
	}

	private setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	deleteTimeEntry(): void {
		this.isEntryFormOpened = false;
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
			if (!this.isNewTrackedTimeValid(this.dayInfo.date, this.draggedTimeEntry.timeValues.timeActual)) {
				this.notificationService.danger('Total actual time should be less than 24 hours.');
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
				error => {
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
		return moment(date).format('dddd') + (this.isToday(date) ? ' (Today)' : '');
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

	private isNewTrackedTimeValid(newDate: string, time: number): boolean {
		let dayInfo = this.calendarService.getDayInfoByDate(newDate);
		let totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'timeActual');
		return totalTrackedTimeForDay + time <= MAX_TIMER_VALUE;
	}
}
