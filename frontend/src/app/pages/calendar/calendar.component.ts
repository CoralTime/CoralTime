import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { CalendarProjectsService } from './calendar-projects.service';
import { CustomSelectItem } from '../../shared/form/multiselect/multiselect.component';
import { CalendarService } from '../../services/calendar.service';
import { User } from '../../models/user';
import { ImpersonationService } from '../../services/impersonation.service';
import { Subscription } from 'rxjs/Subscription';
import { DateUtils } from '../../models/calendar';
import { AuthService } from '../../core/auth/auth.service';
import * as moment from 'moment';

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

@Component({
	selector: 'ct-calendar',
	templateUrl: 'calendar.component.html'
})

export class CalendarComponent implements OnInit, OnDestroy {
	activePeriod: number = 7;
	availablePeriod: number;
	date: string;
	isWeekViewActive: boolean = true;
	firstDayOfWeek: number;
	projectIds: number[] = [];
	projects: CustomSelectItem[] = [];
	showOnlyActive: boolean = true;

	private subscriptionImpersonation: Subscription;

	constructor(public impersonationService: ImpersonationService,
	            private authService: AuthService,
	            private calendarService: CalendarService,
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
				this.date = route.params['date'] ? DateUtils.reformatDate(route.params['date'], 'MM-DD-YYYY') : DateUtils.formatDateToString(new Date());
				this.projectIds = route.params['projectIds'] ? route.params['projectIds'].split(',') : [];
				this.projectIds.forEach((id, index) => { this.projectIds[index] = +id; });
				this.projectsService.filteredProjects = this.projectIds;

				if (route.url.length) {
					this.isWeekViewActive = route.url[0].path !== 'day';
				} else {
					this.isWeekViewActive = true;
				}

				this.setActivePeriod();
			}
		});

		this.route.queryParams.subscribe((params) => {
			this.date = params['date'] ? DateUtils.reformatDate(params['date'], 'MM-DD-YYYY') : DateUtils.formatDateToString(new Date());
		});

		this.loadProjects(this.showOnlyActive);
		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			if (this.authService.isLoggedIn()) {
				this.loadProjects(this.showOnlyActive);
			}
		});
	}

	onResize(event): void {
		let width: number = event.target.innerWidth;
		this.setAvailablePeriod(width);
		this.setActivePeriod();
	}

	setActivePeriod(): void {
		if (this.route.snapshot.children[0].url[0] && this.route.snapshot.children[0].url[0].path === 'day') {
			this.activePeriod = 1;
		} else {
			this.activePeriod = this.availablePeriod;
		}
	}

	setAvailablePeriod(width: number): void {
		if (width < 810 && this.availablePeriod !== 1) {
			this.availablePeriod = 1;
		}
		if (width >= 810 && width < 1300 && this.availablePeriod !== 4) {
			this.availablePeriod = 4;
		}
		if (width >= 1300 && this.availablePeriod !== 7) {
			this.availablePeriod = 7;
		}
	}

	getDatesPeriod(periodMove: number): string {
		if (this.activePeriod === 1) {
			let thisDate = this.moveDate(this.date, periodMove);
			return this.formatDate(thisDate);
		} else {
			let firstDate;
			if (this.activePeriod === 4) {
				firstDate = this.moveDate(this.date, this.activePeriod * periodMove);
			} else {
				firstDate = this.moveDate(this.getWeekBeginning(this.date), this.activePeriod * periodMove);
			}
			let lastDate = this.moveDate(firstDate, this.activePeriod - 1);
			return this.formatDate(firstDate) + ' - ' + this.formatDate(lastDate);
		}
	}

	getWeekBeginning(date: string): string {
		return this.calendarService.getWeekBeginning(date, this.firstDayOfWeek);
	}

	toggleView(toggleToWeek: boolean): void {
		let params = new Object();
		if (this.projectIds && this.projectIds.length) {
			params['projectIds'] = this.projectIds.join(',');
		}
		this.activePeriod = toggleToWeek ? this.availablePeriod : 1;
		this.router.navigate(['calendar', (this.activePeriod === 1) ? 'day' : 'week', params]);
	}

	toggleTimePeriod(period: number): void {
		let params = {};
		if (this.activePeriod === 7) {
			this.date = this.getWeekBeginning(this.date);
		}
		this.date = this.moveDate(this.date, period * this.activePeriod);
		params['date'] = this.formatDateToUrlString(this.date);
		if (this.projectIds && this.projectIds.length) {
			params['projectIds'] = this.projectIds.join(',');
		}
		this.router.navigate(['calendar', (this.activePeriod === 1) ? 'day' : 'week', params]);
	}

	toggleProject(newProjectIds: number[]): void {
		let params = {};

		if (newProjectIds && newProjectIds.length) {
			params['projectIds'] = newProjectIds.join(',');
		}
		if (this.date) {
			params['date'] = this.formatDateToUrlString(this.date);
		}

		this.projectsService.filteredProjects = newProjectIds;
		this.router.navigate(['calendar', (this.activePeriod === 1) ? 'day' : 'week', params]);
	}

	toggleArchivedProjects(): void {
		this.showOnlyActive = !this.showOnlyActive;
		this.loadProjects(this.showOnlyActive);
	}

	ngOnDestroy() {
		this.projectsService.clearProject();
		this.calendarService.isAltPressed = false;
		this.calendarService.dragEffect = 'move';
		this.subscriptionImpersonation.unsubscribe();
	}

	private formatDate(date: string): string {
		let d = moment(date).toDate();
		return MONTHS[d.getMonth()] + ' ' + (d.getDate());
	}

	private formatDateToUrlString(date: string): string {
		let d = moment(date).toDate();
		return (d.getMonth() + 1) + '-' + d.getDate() + '-' + d.getFullYear();
	}

	private loadProjects(showOnlyActive: boolean = true): void {
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
				};
			});
		});
	}

	private moveDate(date: string, dif: number): string {
		let newDate = moment(date).toDate();
		return DateUtils.formatDateToString(newDate.setDate(newDate.getDate() + dif));
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (event.key === 'Alt') {
			this.calendarService.isAltPressed = true;
			this.calendarService.dragEffect = 'copy';
		}
	}

	@HostListener('document:keyup', ['$event'])
	onKeyUp(event: KeyboardEvent) {
		if (event.key === 'Alt') {
			this.calendarService.isAltPressed = false;
			this.calendarService.dragEffect = 'move';
		}
	}
}
