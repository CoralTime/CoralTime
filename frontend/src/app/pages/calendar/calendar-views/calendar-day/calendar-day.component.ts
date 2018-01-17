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

	newTimeEntry: TimeEntry;
	isEntryFormOpened: boolean = false;
	draggedTimeEntry: TimeEntry;
	isDragEnter: boolean;
	fakeCalendarTaskHeight: number;
	canChangeDragEnter: boolean = true;
	changeDragEnterTimeout: any;

	@ViewChild('calendarTask', {read: ElementRef}) calendarTask: ElementRef;

	constructor(private calendarService: CalendarService,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		if (this.isToday(new Date(this.dayInfo.date))) {
			this.calendarService.isTimerActivated = !!this.dayInfo.timeEntries.find((timeEntry) => timeEntry.timeTimerStart > 0);
		}
	}

	setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	getDateString(date: Date): string {
		return moment(date).format('dddd') + (this.isToday(date) ? ' (Today)' : '');
	}

	isToday(date: Date): boolean {
		return (new Date()).toDateString() === date.toDateString();
	}

	calcTime(type: string): string {
		let timeEnries: TimeEntry[] = this.dayInfo.timeEntries;
		let time: number = 0;
		if (timeEnries) {
			for (let i = 0; i < timeEnries.length; i++) {
				time += timeEnries[i][type];
			}
		}

		return this.setTimeString(time);
	}

	addNewTimeEntry(currentDate: Date): void {
		let newTimeEntry = {
			date: currentDate,
			description: '',
			projectName: 'Select Project',
			taskName: 'Select Task'
		};
		this.newTimeEntry = new TimeEntry(newTimeEntry);
		this.isEntryFormOpened = true;
		setTimeout(() => {
			this.entryForm.toggleEntryTimeForm();
		}, 0);
	}

	deleteTimeEntry(): void {
		this.isEntryFormOpened = false;
	}

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
			if (new Date(this.draggedTimeEntry.date).toDateString() === this.dayInfo.date.toDateString()) {
				return;
			}
			if (!this.isNewTrackedTimeValid(this.dayInfo.date, this.draggedTimeEntry.time)) {
				this.notificationService.danger('Total actual time can\'t be more than 24 hours');
				return;
			}
			this.draggedTimeEntry.date = DateUtils.convertMomentToUTC(moment(this.dayInfo.date));
			this.draggedTimeEntry.timeTimerStart = -1;

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
					this.notificationService.danger('Error moving Time Entry');
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

	isAltPressed(): boolean {
		return this.calendarService.isAltPressed;
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

	private isNewTrackedTimeValid(newDate: Date, time: number): boolean {
		let dayInfo = this.calendarService.getDayInfoByDate(newDate);
		let totalTrackedTimeForDay = this.calendarService.getTotalTimeForDay(dayInfo, 'time');
		return totalTrackedTimeForDay + time <= MAX_TIMER_VALUE;
	}
}
