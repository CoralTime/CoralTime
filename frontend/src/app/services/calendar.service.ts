import { Observable } from 'rxjs';
import { Injectable, EventEmitter } from '@angular/core';
import { TimeEntry, CalendarDay } from '../models/calendar';
import { ODataServiceFactory } from './odata/odataservicefactory';
import { ODataService } from './odata/odata';
import { ArrayUtils } from '../core/object-utils';
import { AuthService } from '../core/auth/auth.service';
import { Project } from '../models/project';
import * as moment from 'moment';

@Injectable()
export class CalendarService {
	readonly odata: ODataService<TimeEntry>;

	defaultProject: Project;
	dragEffect: string = 'move';
	draggedTimeEntry: TimeEntry;
	fakeCalendarTaskHeight: number;
	firstDayOfWeek: number;
	isAltPressed: boolean = false;
	isTimerActivated: boolean;
	isTimeEntryFormOpened: boolean = false;
	timeEntriesUpdated: EventEmitter<void> = new EventEmitter<void>();

	calendar: CalendarDay[] = [];

	constructor(private authService: AuthService,
	            private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<TimeEntry>('TimeEntries');
		if (localStorage.hasOwnProperty('DEFAULT_PROJECT')) {
			this.defaultProject = JSON.parse(localStorage.getItem('DEFAULT_PROJECT'));
		}

		this.authService.onChange.subscribe(() => {
			this.setDefaultProject(null);
		});
	}

	getTimeEntries(dateFrom: Date, dif?: number): Observable<TimeEntry[]> {
		let dateTo = this.moveDate(dateFrom, dif || 1);
		let filters = [];
		let query = this.odata.Query();

		let newDateFrom = new Date(new Date(dateFrom).setDate(dateFrom.getDate() - 1));
		let newDateTo = new Date(new Date(dateTo).setDate(dateTo.getDate() - 1));
		filters.push('Date gt ' + moment(newDateFrom).format('YYYY-MM-DD') + 'T17:59:59Z');
		filters.push('Date lt ' + moment(newDateTo).format('YYYY-MM-DD') + 'T18:00:00Z');

		query.Filter(filters.join(' and '));

		return query.Exec().map(res => {
			let timeEntries = this.sortTimeEntries(res);
			return timeEntries.map((x: any) => new TimeEntry(x));
		});
	}

	moveDate(date: Date, dif: number): Date {
		let newDate = new Date(date);
		return new Date(newDate.setDate(date.getDate() + dif));
	}

	getDayInfoByDate(timeEntryDate: Date): CalendarDay {
		return this.calendar.find((day: CalendarDay) => {
			return day.date.getDate() == (new Date(timeEntryDate)).getDate()
		})
	}

	getTotalTimeForDay(day: CalendarDay, timeField: string): number {
		let totalTime: number = 0;
		day && day.timeEntries.forEach((timeEntry: TimeEntry) => {
			totalTime += timeEntry[timeField];
		});

		return totalTime;
	}

	getWeekBeginning(date: Date, firstDayOfWeek: number): Date {
		let firstDayCorrection: number = ((date.getDay() < firstDayOfWeek) ? -7 : 0);
		return new Date(date.setDate(date.getDate() - date.getDay() + firstDayOfWeek + firstDayCorrection));
	}

	setDefaultProject(project: Project): void {
		this.defaultProject = project;
		localStorage.setItem('DEFAULT_PROJECT', JSON.stringify(project));
	}

	private sortTimeEntries(timeEntries: TimeEntry[]): TimeEntry[] {
		let arrayWithFromToPeriod = timeEntries.filter((timeEntry) => timeEntry.isFromToShow == true);
		let otherTimeEntries = timeEntries.filter((timeEntry) => timeEntry.isFromToShow == false);

		ArrayUtils.sortByField(arrayWithFromToPeriod, 'timeFrom');
		ArrayUtils.sortByField(otherTimeEntries, 'id');

		return [...arrayWithFromToPeriod, ...otherTimeEntries];
	}
}