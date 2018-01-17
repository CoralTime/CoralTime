import { Component, OnInit } from '@angular/core';
import {
	ReportsService, ProjectDetail, ReportDropdowns, UserDetail, ReportGrid,
	GroupByItem, ClientDetail, ReportsSettings
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
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-reports',
	templateUrl: 'reports.component.html'
})

export class ReportsComponent implements OnInit {
	reportDropdowns: ReportDropdowns;
	reportsGridData: ReportGrid;

	clientIds: number[];
	clientItems: CustomSelectItem[] = [];
	clients: ClientDetail[] = [];
	selectedClients: ClientDetail[] = [];

	projectIds: number[] = [];
	projects: ProjectDetail[] = [];
	projectItems: CustomSelectItem[] = [];
	selectedProjects: ProjectDetail[] = [];

	userIds: number[] = [];
	users: UserDetail[] = [];
	userItems: CustomSelectItem[] = [];

	groupByItems: GroupByItem[] = [];
	groupModel: GroupByItem;

	isUsersFilterShown: boolean = false;
	showOnlyActiveClients: boolean = true;
	showOnlyActiveProjects: boolean = true;
	showOnlyActiveUsers: boolean = true;

	showColumnItems: CustomSelectItem[];
	showColumnIds: number[];
	showColumns: number[];

	isDatepickerShown: boolean = false;
	isDatepickerAnimating: boolean = false;
	canCloseDatepicker: boolean = true;
	dateFormat: string;
	dateFormatId: number;
	datePeriod: DatePeriod;
	dateString: string = 'This Week';
	firstDayOfWeek: number;
	oldDatePeriod: DatePeriod;
	oldDateString: string;
	userInfo: User;

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
		this.route.data.forEach((data: { user: User }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
			this.dateFormat = this.userInfo.dateFormat;
			this.dateFormatId = this.userInfo.dateFormatId;
			this.firstDayOfWeek = this.userInfo.weekStart;
			this.rangeDatepickerService.setDatePeriodList(this.userInfo.weekStart);
		});
		this.getReportSettings();
		this.isUsersFilterShown = this.authService.isUserAdminOrManager;
		this.getReportDropdowns();
		this.getReportGrid();
	}

	groupByChange(): void {
		this.updateReportSettings();
		this.getReportGrid();
	}

	getReportDropdowns(): void {
		this.reportsService.getReportDropdowns().subscribe((res: ReportDropdowns) => {
			this.reportDropdowns = res;

			this.getClientItems(res.clientsDetails);
			this.getProjectItems(this.clients);
			this.getUserItems(this.projects);
		});
	}

	getReportGrid(): void {
		let filters = {
			dateFrom: this.convertMomentToString(this.datePeriod.dateFrom),
			dateTo: this.convertMomentToString(this.datePeriod.dateTo) || this.convertMomentToString(this.datePeriod.dateFrom),
			projectIds: this.projectIds,
			memberIds: this.userIds,
			clientIds: this.clientIds,
			groupById: this.groupModel ? this.groupModel.groupById : 3
		};

		this.reportsService.getReportGrid(filters).subscribe((res: ReportGrid) => {
				this.reportsGridData = res;
			},
			() => {
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

	private updateReportSettings(): void {
		this.reportsService.setReportSettings({
			clientIds: this.clientIds,
			datePeriod: this.datePeriod,
			groupById: this.groupModel ? this.groupModel.groupById : 3,
			projectIds: this.projectIds,
			showColumnIds: this.showColumnIds,
			userIds: this.userIds,
		});
	}

	// ACTIONS

	cancelUpdatingReportGrid(): void {
		this.dateString = this.oldDateString;
		this.datePeriod = this.oldDatePeriod;
		this.closeRangeDatepicker();
	}

	toggleRangeDatepicker(): void {
		if (this.isDatepickerShown && this.canCloseDatepicker) {
			this.closeRangeDatepicker();
			this.getReportGrid();
		} else {
			this.openRangeDatepicker();
		}
		this.changeCloseParameter();
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

	datePeriodOnChange(datePeriod: DatePeriod): void {
		this.datePeriod = datePeriod;
		this.setDateString(datePeriod);
		this.updateReportSettings();
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
		this.updateReportSettings();
	}

	openSendReportsDialog(): void {
		this.reportsSendRef = this.dialog.open(ReportsSendComponent);
		this.reportsSendRef.componentInstance.model = new SendReportsFormModel({
			clientIds: this.clientIds,
			dateFormatId: this.dateFormatId,
			dateFrom: this.convertMomentToString(this.datePeriod.dateFrom),
			dateTo: this.convertMomentToString(this.datePeriod.dateTo) || this.convertMomentToString(this.datePeriod.dateFrom),
			groupById: this.groupModel ? this.groupModel.groupById : 3,
			memberIds: this.userIds,
			projectIds: this.projectIds,
			showColumnIds: this.showColumnIds
		});
		this.reportsSendRef.componentInstance.dateFormat = this.dateFormat;
		this.reportsSendRef.componentInstance.userInfo = this.userInfo;

		if (this.projectIds.length === 1) {
			this.reportsSendRef.componentInstance.projectName
				= ArrayUtils.findByProperty(this.projectItems, 'value', this.projectIds[0]).label;
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

	resetFilters(): void {
		this.clientIds = [];
		this.projectIds = [];
		this.userIds = [];
		this.datePeriodOnChange(this.rangeDatepickerService.getDatePeriodList()['This Week']);
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.groupById === 3);
		this.toggleClient(this.clientIds);
	}

	private changeCloseParameter(): void {
		this.canCloseDatepicker = false;
		setTimeout(() => this.canCloseDatepicker = true, 0);
	}

	private convertMomentToString(moment: Moment): string {
		return moment ? DateUtils.convertMomentToUTC(moment).toISOString() : null;
	}

	private setDateString(period: DatePeriod): void {
		let selectedRange = new DatePeriod(period.dateFrom, period.dateTo);
		this.dateString = this.rangeDatepickerService.setDateStringPeriod(selectedRange);
	}

	submitSettings(showColumnIds: number[]): void {
		this.showColumns = showColumnIds;
		this.updateReportSettings();
	}

	exportAs(fileTypeId: number): void {
		let filters = {
			clientIds: this.clientIds,
			dateFormatId: this.dateFormatId,
			dateFrom: this.convertMomentToString(this.datePeriod.dateFrom),
			dateTo: this.convertMomentToString(this.datePeriod.dateTo) || this.convertMomentToString(this.datePeriod.dateFrom),
			fileTypeId: fileTypeId,
			groupById: this.groupModel ? this.groupModel.groupById : 3,
			memberIds: this.userIds,
			projectIds: this.projectIds,
			showColumnIds: this.showColumnIds
		};
		this.reportsService.exportAs(filters).subscribe();
	}

	formatDate(utcDate: Moment): string {
		return this.dateFormat ? utcDate.format(this.dateFormat) : utcDate.toDate().toLocaleDateString();
	}

	printPage(): void {
		window.print();
	}

	// FILTERS

	toggleClient(clientIds: number[] = []): void {
		this.selectedClients = [];

		clientIds.forEach((clientId: number) => {
			this.selectedClients.push(this.clients.find((client: ClientDetail) => client.clientId === clientId));
		});

		this.getProjectItems(this.selectedClients.length ? this.selectedClients : this.clients);

		this.projectIds = this.projectIds.filter((projectId: number) => {
			return this.projects.find((project: ProjectDetail) => project.projectId === projectId);
		});

		this.toggleProject(this.projectIds);
	}

	toggleArchivedClients(): void {
		this.showOnlyActiveClients = !this.showOnlyActiveClients;
		this.getClientItems(this.reportDropdowns.clientsDetails);
	}

	toggleProject(projectIds: number[] = []): void {
		this.selectedProjects = [];

		projectIds.forEach((projectId: number) => {
			this.selectedProjects.push(this.projects.find((project: ProjectDetail) => project.projectId === projectId));
		});

		this.getUserItems(this.selectedProjects.length ? this.selectedProjects : this.projects);

		this.userIds = this.userIds.filter((userId: number) => {
			return this.users.find((user: UserDetail) => user.userId === userId);
		});

		this.toggleUser();
	}

	toggleArchivedProjects(): void {
		this.showOnlyActiveProjects = !this.showOnlyActiveProjects;
		this.getProjectItems(this.selectedClients.length ? this.selectedClients : this.reportDropdowns.clientsDetails);

		if (this.reportDropdowns.isAdminCurrentUser || this.reportDropdowns.isManagerCurrentUser) {
			this.getUserItems(this.projects);
		}
	}

	toggleUser(): void {
		this.getReportGrid();
		this.updateReportSettings();
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

		if (!this.reportDropdowns.isAdminCurrentUser && this.reportDropdowns.isManagerCurrentUser) {
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

	private getReportSettings(): void {
		this.setShowColumnItems();
		let reportsSettings: ReportsSettings = this.reportsService.getReportSettings();

		this.showColumnIds = reportsSettings.showColumnIds;
		this.showColumns = this.showColumnIds;
		this.clientIds = reportsSettings.clientIds;
		this.projectIds = reportsSettings.projectIds;
		this.userIds = reportsSettings.userIds;

		if (reportsSettings.datePeriod) {
			this.datePeriodOnChange(reportsSettings.datePeriod);
		} else {
			this.datePeriod = this.rangeDatepickerService.getDatePeriodList()['This Week'];
			this.datePeriodOnChange(this.datePeriod);
		}

		this.reportsService.getReportsGroupBy().subscribe((res: GroupByItem[]) => {
			this.groupByItems = res;
			this.groupModel = this.groupByItems.find((group: GroupByItem) => group.groupById === reportsSettings.groupById);
		});
	}

	private setShowColumnItems(): void {
		this.showColumnItems = [
			new CustomSelectItem('Show Estimated Hours', 1),
			new CustomSelectItem('Show Date', 2),
			new CustomSelectItem('Show Notes', 3),
			new CustomSelectItem('Show Start/Finish Time', 4)
		];
	}
}
