import { Component, OnInit } from '@angular/core';
import {
	ReportsService, ProjectDetail, ReportDropdowns, UserDetail, ReportGrid,
	GroupByItem, ClientDetail, ReportFilters
} from '../../services/reposts.service';
import { CustomSelectItem } from '../../shared/form/multiselect/multiselect.component';
import { ArrayUtils } from '../../core/object-utils';
import { AuthService } from '../../core/auth/auth.service';
import { DateUtils } from '../../models/calendar';
import { DatePeriod, RangeDatepickerService } from './range-datepicker/range-datepicker.service';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../models/user';
import { MdDialog, MdDialogRef } from '@angular/material';
import { ReportsSendComponent, SendReportsFormModel } from './reports-send/reports-send.component';
import { NotificationService } from '../../core/notification.service';
import { ImpersonationService } from '../../services/impersonation.service';
import { ReportsQueryFormComponent } from './reports-query-form/reports-query-form.component';
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-reports',
	templateUrl: 'reports.component.html'
})

export class ReportsComponent implements OnInit {
	reportDropdowns: ReportDropdowns;
	reportsGridData: ReportGrid;
	reportFilters: ReportFilters;

	clientItems: CustomSelectItem[] = [];
	clients: ClientDetail[] = [];
	selectedClients: ClientDetail[] = [];

	projects: ProjectDetail[] = [];
	projectItems: CustomSelectItem[] = [];
	selectedProjects: ProjectDetail[] = [];

	users: UserDetail[] = [];
	userItems: CustomSelectItem[] = [];

	groupByItems: GroupByItem[] = [];
	groupModel: GroupByItem;

	queryItems: ReportFilters[] = [];
	queryModel: ReportFilters;

	isUsersFilterShown: boolean = false;
	showOnlyActiveClients: boolean = true;
	showOnlyActiveProjects: boolean = true;
	showOnlyActiveUsers: boolean = true;

	showColumnItems: CustomSelectItem[];
	showColumnIds: number[];

	isDatepickerShown: boolean = false;
	isDatepickerAnimating: boolean = false;
	canToggleDatepicker: boolean = true;
	dateFormat: string;
	dateFormatId: number;
	datePeriod: DatePeriod;
	dateString: string = 'This Week';
	firstDayOfWeek: number;
	oldDatePeriod: DatePeriod;
	oldDateString: string;
	userInfo: User;

	private reportsQueryRef: MdDialogRef<ReportsQueryFormComponent>;
	private reportsSendRef: MdDialogRef<ReportsSendComponent>;

	constructor(public dialog: MdDialog,
	            private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private rangeDatepickerService: RangeDatepickerService,
	            private reportsService: ReportsService,
	            private route: ActivatedRoute) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User, reportFilters: ReportDropdowns }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
			this.dateFormat = this.userInfo.dateFormat;
			this.dateFormatId = this.userInfo.dateFormatId;
			this.firstDayOfWeek = this.userInfo.weekStart;
			this.rangeDatepickerService.setDatePeriodList(this.userInfo.weekStart);
			this.setReportDropdowns(data.reportFilters);
		});
		this.isUsersFilterShown = this.authService.isUserAdminOrManager;
		this.getReportGrid(!!this.reportFilters.queryId);
	}

	setReportDropdowns(reportDropdowns: ReportDropdowns): void {
		this.reportDropdowns = reportDropdowns;

		this.setReportFilters(reportDropdowns.currentQuery);
		this.setReportsGroupBy(reportDropdowns.values.groupBy);
		this.setReportsQueryItems(reportDropdowns);
		this.setShowColumnItems();

		this.getClientItems(reportDropdowns.values.filters);
		this.getProjectItems(this.clients);
		this.getUserItems(this.projects);
	}

	private setReportsGroupBy(groupByArray: GroupByItem[]): void {
		this.groupByItems = groupByArray;
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === this.reportFilters.groupById);
	}

	private setReportFilters(reportFilters: ReportFilters): void {
		this.reportFilters = new ReportFilters(reportFilters);
		this.showColumnIds = this.reportFilters.showColumnIds || [];

		if (reportFilters.dateFrom && reportFilters.dateTo) {
			this.datePeriodOnChange(new DatePeriod(moment(reportFilters.dateFrom), moment(reportFilters.dateTo)));
		} else {
			this.datePeriod = this.rangeDatepickerService.getDatePeriodList()['This Week'];
			this.datePeriodOnChange(this.datePeriod);
		}
	}

	private setReportsQueryItems(reportDropdowns: ReportDropdowns): void {
		this.queryItems = reportDropdowns.values.customQueries;
		this.queryModel = ArrayUtils.findByProperty(this.queryItems, 'queryId', reportDropdowns.currentQuery.queryId);
	}

	private setShowColumnItems(): void {
		this.showColumnItems = [
			new CustomSelectItem('Show Estimated Hours', 1),
			new CustomSelectItem('Show Date', 2),
			new CustomSelectItem('Show Notes', 3),
			new CustomSelectItem('Show Start/Finish Time', 4)
		];
	}

	getReportGrid(isCustomQuery?: boolean): void {
		this.reportFilters.dateFrom = this.convertMomentToString(this.datePeriod.dateFrom);
		this.reportFilters.dateTo = this.convertMomentToString(this.datePeriod.dateTo)
			|| this.convertMomentToString(this.datePeriod.dateFrom);
		if (!isCustomQuery) {
			this.reportFilters.queryId = null;
			this.reportFilters.queryName = null;
			this.queryModel = null;
		}

		let filters = {
			currentQuery: this.reportFilters
		};

		this.reportsService.getReportGrid(filters).subscribe((res: ReportGrid) => {
				this.reportsGridData = res;
			},
			() => {
				this.notificationService.danger('Error loading reports grid.');
				this.datePeriod = new DatePeriod(null);
			});
	}

	getTimeString(time: number, showDefaultValue: boolean = false): string {
		let m = Math.floor(time / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;

		if (!showDefaultValue && h === 0 && m === 0) {
			return '';
		}

		return (((h > 99) ? ('' + h) : ('00' + h).slice(-2)) + ':' + ('00' + m).slice(-2));
	}

	// QUERY ACTIONS

	openQueryDialog(): void {
		this.reportsQueryRef = this.dialog.open(ReportsQueryFormComponent);
		this.reportsQueryRef.componentInstance.model = this.reportFilters;

		this.reportsQueryRef.componentInstance.onSubmit.subscribe((response) => {
			this.reportsQueryRef.close();
			this.onSubmitQueryForm(response);
		});
	}

	onSubmitQueryForm(isError: boolean): void {
		if (!isError) {
			this.updateQueryItems();
			this.notificationService.success('Reports query has been successfully created.');
		} else {
			this.notificationService.danger('Error creating reports query.');
		}
	}

	deleteQuery(queryModel: ReportFilters): void {
		this.reportsService.deleteQuery(queryModel.queryId)
			.subscribe(() => {
					this.notificationService.success('Report query has been successfully deleted.');
					this.updateQueryItems();
				},
				error => this.notificationService.danger('Error deleting report query.'));
	}

	queryOnChange(queryModel: ReportFilters): void {
		this.setReportFilters(queryModel);
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === this.reportFilters.groupById);

		this.getReportGrid(true);
	}

	private updateQueryItems(): void {
		this.reportsService.getReportDropdowns().subscribe((reportDropdowns: ReportDropdowns) => {
			this.setReportsQueryItems(reportDropdowns);
		});
	}

	// DATEPICKER

	cancelUpdatingReportGrid(): void {
		this.dateString = this.oldDateString;
		this.datePeriod = this.oldDatePeriod;
		this.closeRangeDatepicker();
	}

	closeRangeDatepicker(): void {
		this.isDatepickerShown = false;
		this.isDatepickerAnimating = false;
	}

	openRangeDatepicker(): void {
		this.oldDateString = this.dateString;
		this.oldDatePeriod = this.datePeriod;
		this.isDatepickerShown = true;
		setTimeout(() => this.isDatepickerAnimating = true, 300);
	}

	toggleRangeDatepicker(event?: MouseEvent): void {
		if (event && (<HTMLElement>event.target).classList.contains('fa-times') || !this.canToggleDatepicker) {
			return;
		}

		if (this.isDatepickerShown) {
			this.closeRangeDatepicker();
			this.getReportGrid();
		} else {
			this.openRangeDatepicker();
		}

		this.changeToggleParameter();
	}

	datePeriodOnChange(datePeriod: DatePeriod): void {
		this.datePeriod = datePeriod;
		this.setDateString(datePeriod);
	}

	getNewPeriod(isNext: boolean = true): void {
		let dateFrom = this.datePeriod.dateFrom;
		let dateTo = this.datePeriod.dateTo;

		if (this.rangeDatepickerService.isIntegerNumberOfMonths(this.datePeriod)) {
			let monthInPeriod = isNext ? dateTo.diff(dateFrom, 'month') + 1 : -(dateTo.diff(dateFrom, 'month') + 1);
			this.datePeriod = new DatePeriod(
				moment().year(dateFrom.year()).month(dateFrom.month() + monthInPeriod).date(1),
				moment().year(dateTo.year()).month(dateTo.month() + monthInPeriod + 1).date(0)
			);
		} else {
			let daysInPeriod = isNext ? dateTo.diff(dateFrom, 'days') + 1 : -(dateTo.diff(dateFrom, 'days') + 1);
			this.datePeriod = new DatePeriod(
				moment().year(dateFrom.year()).month(dateFrom.month()).date(dateFrom.date() + daysInPeriod),
				moment().year(dateTo.year()).month(dateTo.month()).date(dateTo.date() + daysInPeriod)
			);
		}

		this.setDateString(this.datePeriod);
		this.getReportGrid();
	}

	private changeToggleParameter(): void {
		this.canToggleDatepicker = false;
		setTimeout(() => this.canToggleDatepicker = true, 300);
	}

	private convertMomentToString(moment: Moment): string {
		return moment ? DateUtils.convertMomentToUTC(moment).toISOString() : null;
	}

	private setDateString(period: DatePeriod): void {
		let selectedRange = new DatePeriod(period.dateFrom, period.dateTo);
		this.dateString = this.rangeDatepickerService.setDateStringPeriod(selectedRange);
	}

	// SEND REPORTS

	openSendReportsDialog(): void {
		this.reportsSendRef = this.dialog.open(ReportsSendComponent);
		this.reportsSendRef.componentInstance.model = new SendReportsFormModel({
			dateFormatId: this.dateFormatId,
			currentQuery: this.reportFilters
		});
		this.reportsSendRef.componentInstance.dateFormat = this.dateFormat;
		this.reportsSendRef.componentInstance.userInfo = this.userInfo;

		if (this.reportFilters.projectIds.length === 1) {
			this.reportsSendRef.componentInstance.projectName
				= ArrayUtils.findByProperty(this.projectItems, 'value', this.reportFilters.projectIds[0]).label;
		}

		this.reportsSendRef.componentInstance.onSubmit.subscribe((event) => {
			this.onSubmitSendForm(event);
		});
	}

	onSubmitSendForm(event): void {
		if (event.status === 200) {
			this.notificationService.success('Report has been successfully sent.');
			this.reportsSendRef.close();
		} else {
			this.notificationService.danger('Error sending reports.');
		}
	}

	// GENERAL

	exportAs(fileTypeId: number): void {
		let filters = {
			dateFormatId: this.userInfo.dateFormatId,
			fileTypeId: fileTypeId,
			currentQuery: this.reportFilters
		};

		this.reportsService.exportAs(filters).subscribe();
	}

	formatDate(utcDate: Moment): string {
		return this.dateFormat ? utcDate.format(this.dateFormat) : utcDate.toDate().toLocaleDateString();
	}

	printPage(): void {
		window.print();
	}

	resetFilters(): void {
		this.queryModel = null;
		this.reportFilters = new ReportFilters({});
		this.datePeriodOnChange(this.rangeDatepickerService.getDatePeriodList()['This Week']);
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === 3);
		this.toggleClient(this.reportFilters.clientIds);
	}

	submitSettings(showColumnIds: number[]): void {
		this.showColumnIds = showColumnIds;
		this.getReportGrid();
	}

	// FILTERS

	groupByChange(): void {
		this.reportFilters.groupById = this.groupModel.id;
		this.getReportGrid();
	}

	toggleClient(clientIds: number[] = []): void {
		this.selectedClients = [];

		clientIds.forEach((clientId: number) => {
			this.selectedClients.push(this.clients.find((client: ClientDetail) => client.clientId === clientId));
		});

		this.getProjectItems(this.selectedClients.length ? this.selectedClients : this.clients);

		this.reportFilters.projectIds = this.reportFilters.projectIds.filter((projectId: number) => {
			return this.projects.find((project: ProjectDetail) => project.projectId === projectId);
		});

		this.toggleProject(this.reportFilters.projectIds);
	}

	toggleArchivedClients(): void {
		this.showOnlyActiveClients = !this.showOnlyActiveClients;
		this.getClientItems(this.reportDropdowns.values.filters);
	}

	toggleProject(projectIds: number[] = []): void {
		this.selectedProjects = [];

		projectIds.forEach((projectId: number) => {
			this.selectedProjects.push(this.projects.find((project: ProjectDetail) => project.projectId === projectId));
		});

		this.getUserItems(this.selectedProjects.length ? this.selectedProjects : this.projects);

		this.reportFilters.memberIds = this.reportFilters.memberIds.filter((userId: number) => {
			return this.users.find((user: UserDetail) => user.userId === userId);
		});

		this.toggleUser();
	}

	toggleArchivedProjects(): void {
		this.showOnlyActiveProjects = !this.showOnlyActiveProjects;
		this.getProjectItems(this.selectedClients.length ? this.selectedClients : this.reportDropdowns.values.filters);

		if (this.reportDropdowns.values.userDetails.isAdminCurrentUser || this.reportDropdowns.values.userDetails.isManagerCurrentUser) {
			this.getUserItems(this.projects);
		}
	}

	toggleUser(): void {
		this.getReportGrid();
	}

	toggleArchivedUsers(): void {
		this.showOnlyActiveUsers = !this.showOnlyActiveUsers;
		this.getUserItems(this.selectedProjects.length ? this.selectedProjects : this.projects);
	}

	private getClientItems(clients: ClientDetail[]): void {
		this.clients = this.showOnlyActiveClients ? clients.filter((client: ClientDetail) => client.isClientActive === true) : clients;
		this.clientItems = this.clients.map((client: ClientDetail) => new CustomSelectItem(client.clientName, client.clientId, client.isClientActive));
		this.clientItems = ArrayUtils.sortByField(this.clientItems, 'label');
	}

	private getProjectsFromClients(clients: ClientDetail[]): ProjectDetail[] {
		let projects = [];

		clients.forEach((client: ClientDetail) => {
			projects = [
				...projects,
				...client.projectsDetails
			];
		});

		return projects;
	}

	private getProjectItems(clients: ClientDetail[]): void {
		let projects: ProjectDetail[] = this.getProjectsFromClients(clients);
		this.projects = this.showOnlyActiveProjects ? projects.filter((project: ProjectDetail) => project.isProjectActive === true) : projects;
		this.projectItems = this.projects.map((project: ProjectDetail) => new CustomSelectItem(project.projectName, project.projectId, project.isProjectActive));
		this.projectItems = ArrayUtils.sortByField(this.projectItems, 'label');
	}

	private getUsersFromProjects(projects: ProjectDetail[]): UserDetail[] {
		let users = [];

		if (!this.reportDropdowns.values.userDetails.isAdminCurrentUser && this.reportDropdowns.values.userDetails.isManagerCurrentUser) {
			projects = projects.filter((project: ProjectDetail) => project.isUserManagerOnProject === true);
		}

		projects.forEach((project: ProjectDetail) => {
			users = [
				...users,
				...project.usersDetails
			];
		});

		return this.getUniqueObjectsByProperty(users, 'userId');
	}

	private getUserItems(projects: ProjectDetail[]): void {
		let users: UserDetail[] = this.getUsersFromProjects(projects);
		this.users = this.showOnlyActiveUsers ? users.filter((user: UserDetail) => user.isUserActive === true) : users;
		this.userItems = this.users.map((user: UserDetail) => new CustomSelectItem(user.userFullName, user.userId, user.isUserActive));
		this.userItems = ArrayUtils.sortByField(this.userItems, 'label');
	}

	private getUniqueObjectsByProperty(arr: any[], prop: string) {
		return arr.filter(function (a) {
			let key = a[prop];
			if (!this[key]) {
				this[key] = true;
				return true;
			}
		}, Object.create(null));
	}
}
