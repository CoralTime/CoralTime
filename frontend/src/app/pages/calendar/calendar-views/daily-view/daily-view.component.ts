import { Component, OnInit, OnDestroy, HostBinding, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs';
import { TimeEntry, CalendarDay, DateUtils } from '../../../../models/calendar';
import { Project } from '../../../../models/project';
import { AuthService } from '../../../../core/auth/auth.service';
import { CalendarService } from '../../../../services/calendar.service';
import { LoadingMaskService } from '../../../../shared/loading-indicator/loading-mask.service';
import { CalendarDayComponent } from '../calendar-day/calendar-day.component';
import { ctCalendarAnimation } from '../../calendar.animation';

@Component({
	templateUrl: 'daily-view.component.html',
	selector: 'ct-calendar-daily-view',
	animations: [ctCalendarAnimation.slideCalendar]
})

export class CalendarDailyViewComponent implements OnInit, OnDestroy {
	@HostBinding('class.ct-calendar-daily-view') addClass: boolean = true;

	animationDisabled: boolean = false;
	animationState: string;
	date: string;
	dayInfo: CalendarDay;
	oldDate: string;
	projectIds: number[];
	projects: Project[] = [];
	projectTimeEntries: TimeEntry[] = [];
	timeEntries: TimeEntry[];

	@ViewChild('calendarDay') calendarDay: CalendarDayComponent;

	private timeEntriesSubscription: Subscription;

	constructor(private authService: AuthService,
	            private calendarService: CalendarService,
	            private loadingService: LoadingMaskService,
	            private route: ActivatedRoute) {
	}

	ngOnInit() {
		this.route.params.subscribe((params: Params) => {
			this.oldDate = this.date || DateUtils.formatDateToString(new Date());
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
		this.animationDisabled = true;
		this.animationState = 'hide';
		this.loadingService.addLoading();
		this.calendarService.getTimeEntries(this.date)
			.finally(() => {
				this.loadingService.removeLoading();
				this.animationDisabled = false;
			})
			.subscribe((res) => {
				this.timeEntries = res;

				if (projectIds) {
					this.filterByProject(projectIds);
				} else {
					this.filterByProject();
				}

				this.setDayInfo();
				this.triggerAnimation(this.oldDate, this.date);
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

	setDayInfo(): void {
		this.dayInfo.timeEntries = this.projectTimeEntries;
		this.calendarService.calendar = [this.dayInfo];
	}

	setDate(): void {
		this.dayInfo = new CalendarDay({date: this.date});
	}

	ngOnDestroy() {
		this.timeEntriesSubscription.unsubscribe();
	}

	private triggerAnimation(oldDate: string, newDate: string): void {
		this.animationState = (new Date(oldDate) <= new Date(newDate)) ? 'left' : 'right';
		this.calendarDay.triggerAnimation();
	}
}
