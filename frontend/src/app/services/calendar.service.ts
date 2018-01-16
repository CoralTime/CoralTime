import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Injectable, EventEmitter } from '@angular/core';
import { TimeEntry, CalendarDay } from '../models/calendar';
import { ArrayUtils } from '../core/object-utils';
import { AuthService } from '../core/auth/auth.service';
import { Project } from '../models/project';
import { ConstantService } from '../core/constant.service';
import * as moment from 'moment';

@Injectable()
export class CalendarService {
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
	            private constantService: ConstantService,
	            private http: Http) {
		if (localStorage.hasOwnProperty('DEFAULT_PROJECT')) {
			this.defaultProject = JSON.parse(localStorage.getItem('DEFAULT_PROJECT'));
		}

		this.authService.onChange.subscribe(() => {
			this.setDefaultProject(null);
		});
	}

	getTimeEntries(dateFrom: Date, dif?: number): Observable<TimeEntry[]> {
		let dateTo = this.moveDate(dateFrom, dif || 1);
		let newDateTo = new Date(new Date(dateTo).setDate(dateTo.getDate() - 1));

		let params = new URLSearchParams();
		params.set('dateBegin', moment(dateFrom).format('YYYY-MM-DD') + 'T00:00:00Z');
		params.set('dateEnd', moment(newDateTo).format('YYYY-MM-DD') + 'T23:59:59Z');

		return this.http.get(this.constantService.timeEntriesApi + '/', {search: params})
			.map((res: Response) => {
				let timeEntries = this.sortTimeEntries(res.json());
				return timeEntries.map((x: any) => new TimeEntry(x))
			})
	}

	Delete(id: string): Observable<TimeEntry[]> {
		return this.http.delete(this.constantService.timeEntriesApi + '(' + id + ')')
			.map((res: Response) => res.json());
	}

	Patch(obj: TimeEntry, id: string): Observable<any> {
		return this.http.patch(this.constantService.timeEntriesApi + '(' + id + ')', obj)
			.map((res: Response) => res.json());
	}

	Post(obj: TimeEntry): Observable<any> {
		return this.http.post(this.constantService.timeEntriesApi + '/', obj)
			.map((res: Response) => res.json());
	}

	Put(obj: TimeEntry, id: string): Observable<TimeEntry[]> {
		return this.http.put(this.constantService.timeEntriesApi + '(' + id + ')', obj)
			.map((res: Response) => res.json());
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