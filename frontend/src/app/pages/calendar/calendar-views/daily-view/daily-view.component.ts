import { Component, OnInit, OnDestroy, HostBinding } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs';
import { TimeEntry, CalendarDay, DateUtils } from '../../../../models/calendar';
import { Project } from '../../../../models/project';
import { AuthService } from '../../../../core/auth/auth.service';
import { CalendarService } from '../../../../services/calendar.service';

@Component({
	templateUrl: 'daily-view.component.html',
	selector: 'ct-calendar-daily-view'
})

export class CalendarDailyViewComponent implements OnInit, OnDestroy {
	@HostBinding('class.ct-calendar-daily-view') addClass: boolean = true;

	date: string;
	dayInfo: CalendarDay;
	projectIds: number[];
	projects: Project[] = [];
	projectTimeEntries: TimeEntry[] = [];
	timeEntries: TimeEntry[];

	private timeEntriesSubscription: Subscription;

	constructor(private authService: AuthService,
	            private calendarService: CalendarService,
	            private route: ActivatedRoute) {
	}

	ngOnInit() {
		this.route.params.subscribe((params: Params) => {
			this.projectIds = params['projectIds'] ? params['projectIds'].split(',') : null;
			this.date = params['date'] ? DateUtils.reformatDate(params['date'], 'MM-DD-YYYY') : DateUtils.formatDateToString(new Date());
			this.setDate();
			this.getTimeEntries(this.projectIds);
		});
		this.timeEntriesSubscription = this.calendarService.timeEntriesUpdated
			.subscribe(() => {
				if (this.authService.isLoggedIn()) {
					this.getTimeEntries(this.projectIds);
				}
			});
	}

	getTimeEntries(projectIds?: number[]) {
		this.calendarService.getTimeEntries(this.date)
			.subscribe((res) => {
				this.timeEntries = res;
				if (projectIds) {
					this.filterByProject(projectIds);
				} else {
					this.filterByProject();
				}
				this.setDayInfo();
			});
	}

	filterByProject(projectIds?: number[]): void {
		if (projectIds && projectIds.length) {
			this.projectTimeEntries = [];
			this.timeEntries.forEach((timeEntry: TimeEntry) => {
				if (projectIds.map(x => +x).indexOf(timeEntry.projectId) !== -1) {
					this.projectTimeEntries.push(timeEntry);
				}
			});
		} else {
			this.projectTimeEntries = this.timeEntries;
		}
	}

	setTimeString(s: number): string {
		let m = Math.floor(s / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;
		return (('00' + h).slice(-2) + ':' + ('00' + m).slice(-2));
	}

	setDayInfo(): void {
		this.dayInfo.timeEntries = this.projectTimeEntries;
	}

	setDate(): void {
		this.dayInfo = new CalendarDay({date: this.date});
	}

	getTotalTime(timeEntries?: TimeEntry[]): string {
		let time = 0;
		if (!timeEntries) {
			return this.setTimeString(time);
		}

		timeEntries.forEach((timeEntry) => {
			time += timeEntry.timeValues['timeActual'];
		});

		return this.setTimeString(time);
	}

	getTotalEstimatedTime(timeEntries?: TimeEntry[]): string {
		let timeEstimated = 0;
		if (!timeEntries) {
			return this.setTimeString(timeEstimated);
		}

		timeEntries.forEach((timeEntry: TimeEntry) => {
			timeEstimated += timeEntry.timeValues['timeEstimated'];
		});

		return this.setTimeString(timeEstimated);
	}

	ngOnDestroy() {
		this.timeEntriesSubscription.unsubscribe();
	}
}
