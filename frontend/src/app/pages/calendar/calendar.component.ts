import { Subscription } from 'rxjs/Subscription';
import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { CalendarProjectsService } from './calendar-projects.service';
import { CustomSelectItem } from '../../shared/form/multiselect/multiselect.component';
import { CalendarService } from '../../services/calendar.service';
import { User } from '../../models/user';
import { ImpersonationService } from '../../services/impersonation.service';
import * as moment from 'moment';

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

@Component({
	selector: 'ct-calendar',
	templateUrl: 'calendar.component.html'
})

export class CalendarComponent implements OnInit, OnDestroy {
	activePeriod: number = 7;
	availablePeriod: number;
	date: Date;
	isWeekViewActive: boolean = true;
	firstDayOfWeek: number;
	projectIds: number[] = [];
	projects: CustomSelectItem[] = [];
	showOnlyActive: boolean = true;

	constructor(private calendarService: CalendarService,
	            private impersonationService: ImpersonationService,
	            private route: ActivatedRoute,
	            private router: Router,
	            private projectsService: CalendarProjectsService) {}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			let user = this.impersonationService.impersonationUser || data.user;
			this.firstDayOfWeek = user.weekStart;
		});
		this.setAvailablePeriod(window.innerWidth);
		this.setActivePeriod();
		this.router.events.subscribe(event => {
			if (event instanceof NavigationEnd) {
				let route = this.route.snapshot.children[0];
				this.date = route.params['date'] ? moment(route.params['date'], 'MM-DD-YYYY').utc().toDate() : moment().startOf('day').toDate();
				this.projectIds = route.params['projectIds'] ? route.params['projectIds'].split(',') : [];
				this.projectIds.forEach((id, index) => {this.projectIds[index] = +id});
				this.projectsService.filteredProjects = this.projectIds;
				if (route.url.length) {
					this.isWeekViewActive = route.url[0].path != 'day';
				} else {
					this.isWeekViewActive = true;
				}
				this.setActivePeriod();
			}
		});

		this.route.queryParams.subscribe((params) => {
			let date: Date = new Date();
			this.date = params['date'] ? new Date(params['date']) : new Date();
		});

		this.loadProjects(this.showOnlyActive);
	}

	onResize(event): void {
		let width: number = event.target.innerWidth;
		this.setAvailablePeriod(width);
		this.setActivePeriod();
	}

	setActivePeriod(): void {
		if (this.route.snapshot.children[0].url[0] && this.route.snapshot.children[0].url[0].path == 'day') {
			this.activePeriod = 1;
		} else {
			this.activePeriod = this.availablePeriod
		}
	}

	setAvailablePeriod(width: number): void {
		if (width < 810 && this.availablePeriod != 1) {
			this.availablePeriod = 1;
		}
		if (width >= 810 && width < 1300 && this.availablePeriod != 4) {
			this.availablePeriod = 4;
		}
		if (width >= 1300 && this.availablePeriod != 7) {
			this.availablePeriod = 7;
		}
	}

	getDatesPeriod(periodMove: number): string {
		if (this.activePeriod == 1) {
			let thisDate = this.moveDate(this.date, periodMove);
			return this.formatDate(thisDate);
		} else {
			let firstDate;
			if (this.activePeriod == 4) {
				firstDate = this.moveDate(this.date, this.activePeriod * periodMove);
			} else {
				firstDate = this.moveDate(this.getWeekBeginning(this.date), this.activePeriod * periodMove);
			}
			let lastDate = this.moveDate(firstDate, this.activePeriod - 1);
			return this.formatDate(firstDate) + ' - ' + this.formatDate(lastDate);
		}
	}

	getWeekBeginning(date: Date): Date {
		return this.calendarService.getWeekBeginning(date, this.firstDayOfWeek);
	}

	moveDate(date: Date, dif: number): Date {
		let newDate = new Date(date);
		return new Date(newDate.setDate(date.getDate() + dif));
	}

	dateToString(date: Date): string {
		return (date.getMonth() + 1) + '-' + date.getDate() + '-' + date.getFullYear();
	}

	toggleView(toggleToWeek: boolean): void {
		let params = new Object();
		if (this.projectIds && this.projectIds.length) {
			params['projectIds'] = this.projectIds.join(',');
		}
		this.activePeriod = toggleToWeek ? this.availablePeriod : 1;
		this.router.navigate(['calendar', (this.activePeriod == 1) ? 'day' : 'week', params]);
	}

	toggleTimePeriod(period: number): void {
		let params = {};
		if (this.activePeriod == 7) {
			this.date = this.getWeekBeginning(this.date)
		}
		this.date = this.moveDate(this.date, period * this.activePeriod);
		params['date'] = this.dateToString(this.date);
		if (this.projectIds && this.projectIds.length) {
			params['projectIds'] = this.projectIds.join(',');
		}
		this.router.navigate(['calendar', (this.activePeriod == 1) ? 'day' : 'week', params]);
	}

	toggleProject(newProjectIds: number[]): void {
		let params = {};

		if (newProjectIds && newProjectIds.length) {
			params['projectIds'] = newProjectIds.join(',');
		}
		if (this.date) {
			params['date'] = this.dateToString(this.date);
		}

		this.projectsService.filteredProjects = newProjectIds;
		this.router.navigate(['calendar', (this.activePeriod == 1) ? 'day' : 'week', params]);
	}

	loadProjects(showOnlyActive: boolean = true): void {
		this.projectsService.loadProjects(showOnlyActive).subscribe((res) => {
			this.projectIds = this.projectIds.filter(projectId => {
				return res.find(project => {
					return project.id === projectId;
				});
			});
			this.projects = res.map(project => {
				return {
					isActive: project.isActive,
					value: project.id,
					label: project.name
				}
			});
		});
	}

	toggleArchivedProjects(): void {
		this.showOnlyActive = !this.showOnlyActive;
		this.loadProjects(this.showOnlyActive);
	}

	ngOnDestroy() {
		this.projectsService.clearProject();
		this.calendarService.isAltPressed = false;
		this.calendarService.dragEffect = 'move';
	}

	private formatDate(date: Date): string {
		return MONTHS[date.getMonth()] + ' ' + (date.getDate());
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (event.key == 'Alt') {
			this.calendarService.isAltPressed = true;
			this.calendarService.dragEffect = 'copy';
		}
	}

	@HostListener('document:keyup', ['$event'])
	onKeyUp(event: KeyboardEvent) {
		if (event.key == 'Alt') {
			this.calendarService.isAltPressed = false;
			this.calendarService.dragEffect = 'move';
		}
	}
}
