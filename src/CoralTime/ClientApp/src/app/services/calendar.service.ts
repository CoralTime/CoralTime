import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import * as moment from 'moment';
import { TimeEntry, CalendarDay, DateUtils, TimerResponse } from '../models/calendar';
import { ArrayUtils } from '../core/object-utils';
import { ConstantService } from '../core/constant.service';

@Injectable()
export class CalendarService {
	dragEffect: string = 'move';
	draggedTimeEntry: TimeEntry;
	fakeCalendarTaskHeight: number;
	firstDayOfWeek: number;
	isAltPressed: boolean = false;
	isTimeEntryFormOpened: boolean = false;
	timeEntriesUpdated: EventEmitter<void> = new EventEmitter<void>();

	calendar: CalendarDay[] = [];

	constructor(private constantService: ConstantService,
	            private http: HttpClient) {
	}

	getTimeEntries(dateFrom: string, dif?: number): Observable<TimeEntry[]> {
		const dateTo = moment(this.moveDate(dateFrom, dif || 1)).toDate();
		const newDateTo = DateUtils.formatDateToString(dateTo.setDate(dateTo.getDate() - 1));

		const params = {
			'dateBegin': dateFrom + 'T00:00:00Z',
			'dateEnd': newDateTo + 'T23:59:59Z'
		};

		return this.http.get(this.constantService.timeEntriesApi, {params: params})
			.map((res: TimeEntry[]) => {
				let timeEntries = this.sortTimeEntries(res);
				return timeEntries.map((x: any) => new TimeEntry(x)).filter((x) => x.timeOptions.timeTimerStart <= 0)
			});
	}

	Delete(id: string): Observable<any> {
		return this.http.delete(this.constantService.timeEntriesApi + id);
	}

	Post(obj: TimeEntry): Observable<TimeEntry> {
		return this.http.post(this.constantService.timeEntriesApi, obj)
			.map((res: Object) => new TimeEntry(res));
	}

	Put(obj: TimeEntry, id: string): Observable<TimeEntry> {
		return this.http.put(this.constantService.timeEntriesApi + id, obj)
			.map((res: Object) => new TimeEntry(res));
	}

	getDayInfoByDate(timeEntryDate: string): CalendarDay {
		return this.calendar.find((day: CalendarDay) => {
			return moment(day.date).toDate().getDate() === moment(timeEntryDate).toDate().getDate();
		});
	}

	getTimer(): Observable<TimerResponse> {
		const date = DateUtils.formatDateToString(new Date());
		return this.http.get(this.constantService.timeEntriesApi + 'TimeEntryTimer?date=' + date)
			.map((res: Object) => new TimerResponse(res));
	}

	getTotalTimeForDay(day: CalendarDay, timeField: string): number {
		let totalTime: number = 0;
		day && day.timeEntries.forEach((timeEntry: TimeEntry) => {
			totalTime += timeEntry.timeValues[timeField];
		});

		return totalTime;
	}

	getWeekBeginning(date: string, firstDayOfWeek: number): string {
		const thisDate = moment(date).toDate();
		const firstDayCorrection = (thisDate.getDay() < firstDayOfWeek) ? -7 : 0;
		const dayCorrection = thisDate.setDate(thisDate.getDate() - thisDate.getDay() + firstDayOfWeek + firstDayCorrection);
		return DateUtils.formatDateToString(new Date(dayCorrection));
	}

	private moveDate(date: string, dif: number): string {
		const newDate = moment(date).toDate();
		return DateUtils.formatDateToString(newDate.setDate(newDate.getDate() + dif));
	}

	private sortTimeEntries(timeEntries: TimeEntry[]): TimeEntry[] {
		const arrayWithFromToPeriod = timeEntries.filter((timeEntry) => timeEntry.timeOptions.isFromToShow === true);
		const otherTimeEntries = timeEntries.filter((timeEntry) => timeEntry.timeOptions.isFromToShow === false);

		ArrayUtils.sortByField(arrayWithFromToPeriod, 'timeValues', 1, 'timeFrom');
		ArrayUtils.sortByField(otherTimeEntries, 'id');

		return [...arrayWithFromToPeriod, ...otherTimeEntries];
	}
}
