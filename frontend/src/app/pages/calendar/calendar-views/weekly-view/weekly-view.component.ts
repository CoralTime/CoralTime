import { Component, OnInit, OnDestroy, HostBinding } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs';
import * as moment from 'moment';
import { TimeEntry, CalendarDay, DateUtils } from '../../../../models/calendar';
import { Project } from '../../../../models/project';
import { User } from '../../../../models/user';
import { AuthService } from '../../../../core/auth/auth.service';
import { CalendarService } from '../../../../services/calendar.service';
import { ImpersonationService } from '../../../../services/impersonation.service';
import { LoadingMaskService } from '../../../../shared/loading-indicator/loading-mask.service';
import { ctCalendarAnimation } from '../../calendar.animation';

@Component({
	selector: 'ct-calendar-weekly-view',
	templateUrl: 'weekly-view.component.html',
	animations: [ctCalendarAnimation.slideCalendar]
})

export class CalendarWeeklyViewComponent implements OnInit, OnDestroy {
	@HostBinding('class.ct-calendar-weekly-view') addClass: boolean = true;

	animationState: string;
	animationDisabled: boolean = false;
	animationDelayArray: number[] = [];
	calendar: CalendarDay[];
	date: string;
	daysInCalendar: number;
	firstDayOfWeek: number;
	oldDate: string;
	projects: Project[] = [];
	projectIds: number[];
	projectTimeEntries: TimeEntry[] = [];
	startDay: string;
	endDay: string;
	timeEntries: TimeEntry[];

	private subscriptionImpersonation: Subscription;
	private timeEntriesSubscription: Subscription;

	constructor(private authService: AuthService,
	            private calendarService: CalendarService,
	            private impersonationService: ImpersonationService,
	            private loadingService: LoadingMaskService,
	            private route: ActivatedRoute) {
	}

	ngOnInit() {
		this.calendar = this.setEmptyWeek();

		this.route.data.forEach((data: { user: User }) => {
			let user = this.impersonationService.impersonationUser || data.user;
			this.firstDayOfWeek = user.weekStart;
		});

		this.route.params.subscribe((params: Params) => {
			this.oldDate = this.date || DateUtils.formatDateToString(new Date());
			this.date = params['date'] ? DateUtils.reformatDate(params['date'], 'MM-DD-YYYY') : DateUtils.formatDateToString(new Date());
			this.projectIds = params['projectIds'] ? params['projectIds'].split(',') : null;

			this.triggerAnimation(this.oldDate, this.date);
			this.setAvailablePeriod(window.innerWidth);
			this.getTimeEntries(this.projectIds, params['date'] && this.oldDate === this.date);
		});

		this.timeEntriesSubscription = this.calendarService.timeEntriesUpdated
			.subscribe(() => {
				this.getTimeEntries(this.projectIds, true);
			});
		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			if (this.authService.isLoggedIn()) {
				this.getTimeEntries(this.projectIds);
			}
		});
	}

	getTimeEntries(projIds?: number[], disableAnimation: boolean = false): void {
		this.animationDisabled = disableAnimation;
		this.loadingService.addLoading();
		this.calendarService.getTimeEntries(this.startDay, this.daysInCalendar)
			.finally(() => {
				this.loadingService.removeLoading();
				setTimeout(() => this.animationDisabled = false, 500);
			})
			.subscribe((res) => {
				this.timeEntries = res;

				if (projIds) {
					this.filterByProject(projIds);
				} else {
					this.filterByProject();
				}

				this.sortTimeEntriesByDate();
			});
	}

	getWeekBeginning(date: string): string {
		return this.calendarService.getWeekBeginning(date, this.firstDayOfWeek);
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

	onResize(event): void {
		let width: number = event.target.innerWidth;
		let lastPeriod: number = this.daysInCalendar;
		this.setAvailablePeriod(width);
		if (lastPeriod - this.daysInCalendar) {
			this.getTimeEntries(this.projectIds);
		}
	}

	setAvailablePeriod(width: number): void {
		let oldNumberOfDays = this.daysInCalendar;
		if (width < 810 && oldNumberOfDays !== 1) {
			this.daysInCalendar = 1;
		}
		if (width >= 810 && width < 1300 && oldNumberOfDays !== 4) {
			this.daysInCalendar = 4;
		}
		if (width >= 1300 && oldNumberOfDays !== 7) {
			this.daysInCalendar = 7;
		}
		if (this.daysInCalendar !== 7) {
			this.startDay = this.date;
		} else {
			this.startDay = this.getWeekBeginning(this.date);
		}
		this.endDay = this.moveDate(moment(this.startDay).format('YYYY-MM-DD'), this.daysInCalendar - 1);
	}

	setEmptyWeek(): CalendarDay[] {
		let newCalendar: CalendarDay[] = [];
		let newDay: CalendarDay;

		for (let i = 0; i < this.daysInCalendar; i++) {
			newDay = new CalendarDay({date: this.moveDate(DateUtils.formatDateToString(this.startDay), i)});
			newCalendar.push(newDay);
		}

		return newCalendar;
	}

	sortTimeEntriesByDate(): void {
		let newCalendar: CalendarDay[] = this.setEmptyWeek();

		this.projectTimeEntries.forEach((timeEntry: TimeEntry) => {
			newCalendar.forEach((day: CalendarDay) => {
				if (moment(day.date).toDate().getDate() === moment(timeEntry.date).toDate().getDate()) {
					day.timeEntries.push(timeEntry);
				}
			});
		});

		this.setAnimationDelayArray(newCalendar);
		this.setCalendar(newCalendar);
	}

	ngOnDestroy() {
		this.subscriptionImpersonation.unsubscribe();
		this.timeEntriesSubscription.unsubscribe();
	}

	private moveDate(date: string, dif: number): string {
		let newDate = moment(date).toDate();
		return DateUtils.formatDateToString(newDate.setDate(newDate.getDate() + dif));
	}

	private isDayNotChanged(oldDay: CalendarDay, newDay: CalendarDay): boolean {
		return !!oldDay && oldDay.timeEntries.length === newDay.timeEntries.length &&
			oldDay.timeEntries.every((item, i) => item.id === newDay.timeEntries[i].id);
	}

	private setAnimationDelayArray(newCalendar: CalendarDay[]): void {
		let delay: number = 500;
		this.animationDelayArray = [];

		newCalendar.forEach((day: CalendarDay, i) => {
			if (!this.isDayNotChanged(this.calendar[i], day)) {
				this.animationDelayArray[i] = delay;
				delay += 100;
			}
		});
	}

	private setCalendar(newCalendar: CalendarDay[]): void {
		if (newCalendar.length === this.calendar.length) {
			newCalendar.forEach((day, i) => {
				if (this.calendar[i].date !== day.date || this.animationDelayArray[i] > 0) {
					this.calendar[i] = day;
				}
			});
		} else {
			this.calendar = newCalendar;
		}

		this.calendarService.calendar = newCalendar;
	}

	private triggerAnimation(oldDate: string, newDate: string): void {
		this.animationState = (new Date(oldDate) <= new Date(newDate)) ? 'left' : 'right';
	}
}
