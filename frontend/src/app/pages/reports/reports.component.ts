import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import Moment = moment.Moment;
import { DateUtils } from '../../models/calendar';
import {
	ProjectDetail,
	ReportDropdowns,
	UserDetail,
	ReportGrid,
	ReportQuery,
	ClientDetail,
	GroupByItem,
	ShowColumn,
	ReportFiltersRequest,
	ReportGridView,
} from '../../models/reports';
import { User } from '../../models/user';
import { ArrayUtils } from '../../core/object-utils';
import { AuthService } from '../../core/auth/auth.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';
import { NotificationService } from '../../core/notification.service';
import { DatePeriod, DateResponse, RangeDatepickerService } from './range-datepicker/range-datepicker.service';
import { ImpersonationService } from '../../services/impersonation.service';
import { ReportsService, } from '../../services/reposts.service';
import { ConfirmationComponent } from '../../shared/confirmation/confirmation.component';
import { CustomSelectItem } from '../../shared/form/multiselect/multiselect.component';
import { ReportGridData } from './reports-data/reports-grid.component';
import { ReportsSendComponent, SendReportsFormModel } from './reports-send/reports-send.component';
import { ReportsQueryFormComponent } from './reports-query-form/reports-query-form.component';

const ROWS_TOTAL_NUMBER = 50;

@Component({
	selector: 'ct-reports',
	templateUrl: 'reports.component.html'
})

export class ReportsComponent implements OnInit {
	reportDropdowns: ReportDropdowns;
	reportGridData: ReportGrid;
	reportQuery: ReportQuery;

	isGridLoading: boolean = false;
	gridData: ReportGridData[] = [];

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

	queryItems: ReportQuery[] = [];
	queryModel: ReportQuery;

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
	dateResponse: DateResponse;
	dateString: string = 'This Week';
	firstDayOfWeek: number;
	oldDateResponse: DateResponse;
	oldDateString: string;
	userInfo: User;

	@ViewChild('scrollContainer') private scrollContainer: ElementRef;
	@ViewChild('slimScroll') slimScroll: any;

	private chartWidthParam: number;
	private numberOfWorkingDays: number;
	private reportsConfirmationRef: MatDialogRef<ConfirmationComponent>;
	private reportsQueryRef: MatDialogRef<ReportsQueryFormComponent>;
	private reportsSendRef: MatDialogRef<ReportsSendComponent>;

	constructor(private authService: AuthService,
	            private dialog: MatDialog,
	            private impersonationService: ImpersonationService,
	            private loadingService: LoadingMaskService,
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
		});
		this.isUsersFilterShown = this.authService.isUserAdminOrManager;

		this.loadingService.addLoading();
		this.reportsService.getReportDropdowns()
			.finally(() => this.loadingService.removeLoading())
			.subscribe((reportFilters: ReportDropdowns) => {
				this.setReportDropdowns(reportFilters);
				this.getReportGrid(!!this.reportQuery.queryId);
				this.onResize();
			});
	}

	onResize(): void {
		setTimeout(() => {
			this.slimScroll.getBarHeight();
		}, 0);
	}

	setReportDropdowns(reportDropdowns: ReportDropdowns): void {
		this.reportDropdowns = reportDropdowns;
		this.rangeDatepickerService.dateStaticList = reportDropdowns.values.dateStatic;

		this.setReportFilters(reportDropdowns.currentQuery);
		this.setReportGroupBy(reportDropdowns.values.groupBy);
		this.setReportQueryItems(reportDropdowns);
		this.setShowColumnItems(reportDropdowns.values.showColumns);

		this.getClientItems(reportDropdowns.values.filters);
		this.getProjectItems(this.clients);
		this.getUserItems(this.projects);
	}

	private setReportGroupBy(groupByArray: GroupByItem[]): void {
		this.groupByItems = groupByArray;
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === this.reportQuery.groupById);
	}

	private setReportFilters(reportFilters: ReportQuery): void {
		this.reportQuery = new ReportQuery(reportFilters);
		this.showColumnIds = this.reportQuery.showColumnIds || [];

		this.datePeriodOnChange({
			datePeriod: new DatePeriod(moment(reportFilters.dateFrom), moment(reportFilters.dateTo)),
			dateStaticId: reportFilters.dateStaticId
		});
	}

	private setReportQueryItems(reportDropdowns: ReportDropdowns): void {
		this.queryItems = reportDropdowns.values.customQueries;
		this.queryModel = ArrayUtils.findByProperty(this.queryItems, 'queryId', reportDropdowns.currentQuery.queryId);
	}

	private setShowColumnItems(showColumns: ShowColumn[]): void {
		this.showColumnItems = showColumns.map((col: ShowColumn) => new CustomSelectItem(col.description, col.id));
	}

	// GRID DISPLAYING

	getReportGrid(isCustomQuery?: boolean): void {
		this.reportQuery.dateFrom = ReportsComponent.convertMomentToString(this.dateResponse.datePeriod.dateFrom);
		this.reportQuery.dateTo = ReportsComponent.convertMomentToString(this.dateResponse.datePeriod.dateTo);
		this.reportQuery.dateStaticId = this.dateResponse.dateStaticId;

		if (!isCustomQuery) {
			this.reportQuery.queryId = null;
			this.reportQuery.queryName = null;
			this.queryModel = null;
		}

		const filters: ReportFiltersRequest = {
			currentQuery: this.reportQuery,
			date: DateUtils.formatDateToString(new Date()),
		};

		this.loadingService.addLoading();
		this.reportsService.getReportGrid(filters)
			.finally(() => this.loadingService.removeLoading())
			.subscribe((res: ReportGrid) => {
					this.reportGridData = res;
					this.gridData = this.getNextGridDataPage(this.reportGridData.groupedItems, []);
					this.displayChart(this.reportQuery.groupById === 2);
				},
				() => {
					this.notificationService.danger('Error loading reports grid.');
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

	private isAllGridRowsShown(gridDataShown: ReportGridData[]): boolean {
		const gridData = this.reportGridData.groupedItems;
		return gridDataShown.length === gridData.length
			&& gridDataShown[gridDataShown.length - 1].rows === this.getRowsNumberFromGrid([gridData[gridData.length - 1]]);
	}

	private getNextGridDataPage(gridData: ReportGridView[], gridDataToShow: ReportGridData[]): ReportGridData[] {
		if (this.isAllGridRowsShown(gridDataToShow)) {
			return gridDataToShow;
		}

		let gridNumber = gridDataToShow.length - 1;
		let rowsInGrid = gridDataToShow[gridNumber] ? gridDataToShow[gridNumber].rows : 0;
		let rowsLoaded: number = 0;

		// when some rows in last grid already loaded
		if (gridDataToShow[gridNumber] && rowsInGrid < gridData[gridNumber].items.length) {
			gridDataToShow[gridNumber].rows = Math.min(gridData[gridNumber].items.length, rowsInGrid + ROWS_TOTAL_NUMBER);
			rowsLoaded += gridDataToShow[gridNumber].rows - rowsInGrid;
		}
		gridNumber++;

		// when full grid can be loaded
		while (gridNumber < gridData.length && rowsLoaded + gridData[gridNumber].items.length < ROWS_TOTAL_NUMBER) {
			gridDataToShow.push({
				gridData: gridData[gridNumber],
				rows: gridData[gridNumber].items.length
			});
			rowsLoaded += gridDataToShow[gridNumber].rows;
			gridNumber++;
		}

		// load the rest rows of last grid
		if (gridNumber + 1 <= gridData.length) {
			gridDataToShow.push({
				gridData: gridData[gridNumber],
				rows: ROWS_TOTAL_NUMBER - rowsLoaded
			});
		}

		return gridDataToShow;
	}

	private getRowsNumberFromGrid(gridData: ReportGridView[]): number {
		if (!gridData[0]) {
			return 0;
		}

		let rowsNumber: number = 0;
		gridData.forEach((grid: ReportGridView) => {
			rowsNumber += grid.items.length;
		});

		return rowsNumber;
	}

	private showAllReportsGrid(gridData: ReportGridView[]): ReportGridData[] {
		if (this.isAllGridRowsShown(this.gridData)) {
			return this.gridData;
		}

		const gridDataToShow = [];
		gridData.forEach((grid: ReportGridView) => {
			gridDataToShow.push({
				gridData: grid,
				rows: grid.items.length
			});
		});

		return gridDataToShow;
	}

	@HostListener('window:scroll')
	onWindowScroll(): void {
		if (!this.isGridLoading && !this.isAllGridRowsShown(this.gridData)
			&& window.scrollY > this.scrollContainer.nativeElement.offsetHeight - window.innerHeight - 20) {
			this.isGridLoading = true;

			setTimeout(() => {
				this.getNextGridDataPage(this.reportGridData.groupedItems, this.gridData);
				this.isGridLoading = false;
			}, 0);
		}
	}

	// QUERY ACTIONS

	openQueryDialog(): void {
		this.reportsQueryRef = this.dialog.open(ReportsQueryFormComponent);
		this.reportsQueryRef.componentInstance.model = this.reportQuery;

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

	deleteQuery(queryModel: ReportQuery): void {
		this.loadingService.addLoading();
		this.reportsService.deleteQuery(queryModel.queryId)
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Report query has been successfully deleted.');
					this.updateQueryItems();
				},
				() => this.notificationService.danger('Error deleting report query.'));
	}

	queryOnChange(queryModel: ReportQuery): void {
		this.setReportFilters(queryModel);
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === this.reportQuery.groupById);

		this.getReportGrid(true);
	}

	private updateQueryItems(): void {
		this.loadingService.addLoading();
		this.reportsService.getReportDropdowns()
			.finally(() => this.loadingService.removeLoading())
			.subscribe((reportDropdowns: ReportDropdowns) => {
				this.setReportQueryItems(reportDropdowns);
			});
	}

	// DATEPICKER

	cancelUpdatingReportGrid(): void {
		this.dateString = this.oldDateString;
		this.dateResponse = this.oldDateResponse;
		this.closeRangeDatepicker();
	}

	closeRangeDatepicker(): void {
		this.isDatepickerShown = false;
		this.isDatepickerAnimating = false;
	}

	openRangeDatepicker(): void {
		this.oldDateString = this.dateString;
		this.oldDateResponse = this.dateResponse;
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

	datePeriodOnChange(dateResponse: DateResponse): void {
		this.dateResponse = dateResponse;
		this.setDateString(dateResponse.datePeriod);
	}

	getNewPeriod(isNext: boolean = true): void {
		const dateFrom = this.dateResponse.datePeriod.dateFrom;
		const dateTo = this.dateResponse.datePeriod.dateTo;

		if (this.rangeDatepickerService.isIntegerNumberOfMonths(this.dateResponse.datePeriod)) {
			let monthInPeriod = isNext ? dateTo.diff(dateFrom, 'month') + 1 : -(dateTo.diff(dateFrom, 'month') + 1);
			this.dateResponse.datePeriod = new DatePeriod(
				moment().year(dateFrom.year()).month(dateFrom.month() + monthInPeriod).date(1),
				moment().year(dateTo.year()).month(dateTo.month() + monthInPeriod + 1).date(0)
			);
		} else {
			let daysInPeriod = isNext ? dateTo.diff(dateFrom, 'days') + 1 : -(dateTo.diff(dateFrom, 'days') + 1);
			this.dateResponse.datePeriod = new DatePeriod(
				moment().year(dateFrom.year()).month(dateFrom.month()).date(dateFrom.date() + daysInPeriod),
				moment().year(dateTo.year()).month(dateTo.month()).date(dateTo.date() + daysInPeriod)
			);
		}

		this.setDateString(this.dateResponse.datePeriod);
		this.dateResponse.dateStaticId = null;
		this.getReportGrid();
	}

	private changeToggleParameter(): void {
		this.canToggleDatepicker = false;
		setTimeout(() => this.canToggleDatepicker = true, 300);
	}

	private static convertMomentToString(moment: Moment): string {
		return moment ? DateUtils.formatDateToString(moment) : null;
	}

	private setDateString(period: DatePeriod): void {
		let selectedRange = new DatePeriod(period.dateFrom, period.dateTo);
		this.dateString = this.rangeDatepickerService.setDateStringPeriod(selectedRange);
	}

	// SEND REPORTS

	openSendReportsDialog(): void {
		if (this.reportGridData.timeTotal.timeActualTotal === 0) {
			this.notificationService.danger('There is no data to export.');
			return;
		}

		this.reportsSendRef = this.dialog.open(ReportsSendComponent);
		this.reportsSendRef.componentInstance.model = new SendReportsFormModel({
			dateFormatId: this.dateFormatId,
			currentQuery: this.reportQuery
		});
		this.reportsSendRef.componentInstance.dateFormat = this.dateFormat;
		this.reportsSendRef.componentInstance.userInfo = this.userInfo;

		if (this.reportQuery.projectIds.length === 1) {
			this.reportsSendRef.componentInstance.projectName
				= ArrayUtils.findByProperty(this.projectItems, 'value', this.reportQuery.projectIds[0]).label;
		}

		this.reportsSendRef.componentInstance.onSubmit.subscribe((event) => {
			this.reportsSendRef.close();
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

	checkDataAndPrintPage(): void {
		if (this.reportGridData.timeTotal.timeActualTotal === 0) {
			this.notificationService.danger('There is no data to print.');
			return;
		}

		if (this.getRowsNumberFromGrid(this.reportGridData.groupedItems) > 300) {
			this.openConfirmationDialog();
		} else {
			this.printPage();
		}
	}

	private openConfirmationDialog(): void {
		this.reportsConfirmationRef = this.dialog.open(ConfirmationComponent);
		this.reportsConfirmationRef.componentInstance.message = 'Too much data can lead to display problems. Would you like to continue?';

		this.reportsConfirmationRef.componentInstance.onSubmit.subscribe((confirm: boolean) => {
			if (confirm) {
				this.printPage();
			}

			this.reportsConfirmationRef.close();
		});
	}

	private printPage(): void {
		this.gridData = this.showAllReportsGrid(this.reportGridData.groupedItems);
		setTimeout(() => window.print(), 300);
	}

	exportAs(fileTypeId: number): void {
		if (this.reportGridData.timeTotal.timeActualTotal === 0) {
			this.notificationService.danger('There is no data to export.');
			return;
		}

		const filters: ReportFiltersRequest = {
			currentQuery: this.reportQuery,
			date: DateUtils.formatDateToString(new Date()),
			dateFormatId: this.userInfo.dateFormatId,
			fileTypeId: fileTypeId,
		};

		this.loadingService.addLoading();
		this.reportsService.exportAs(filters)
			.finally(() => this.loadingService.removeLoading())
			.subscribe();
	}

	formatDate(utcDate: Moment): string {
		if (!utcDate) {
			return;
		}

		const date = moment(utcDate);
		return this.dateFormat ? date.format(this.dateFormat) : date.toDate().toLocaleDateString();
	}

	resetFilters(): void {
		this.queryModel = null;
		this.reportQuery = new ReportQuery({});
		const defaultDateStaticId = 2;
		const period = this.reportDropdowns.values.dateStatic.find(x=> x.id === defaultDateStaticId);
		
		const dateResponse = {
			datePeriod: new DatePeriod(moment(period.dateFrom), moment(period.dateTo)),
			dateStaticId: defaultDateStaticId
		};
		this.datePeriodOnChange(dateResponse);
		this.groupModel = this.groupByItems.find((group: GroupByItem) => group.id === 3);
		this.toggleClient(this.reportQuery.clientIds);
	}

	submitSettings(showColumnIds: number[]): void {
		this.showColumnIds = showColumnIds;
		this.getReportGrid();
	}

	// DISPLAY CHART

	calcTotalActualTime(hoursPerDay: number): number {
		return this.numberOfWorkingDays * (hoursPerDay || 8) * 3600;
	}

	calcTrackedHours(time: number): number {
		return +(time / 3600).toFixed(0);
	}

	getChartWidth(value: number): string {
		return (value || 8) * this.chartWidthParam + 'px';
	}

	private calcMaxTotalValue(isGroupByUser: boolean): number {
		let arr: number[];

		if (isGroupByUser) {
			arr = this.reportGridData.groupedItems.map(x => x.groupByType.workingHoursPerDay || 8);
		} else {
			arr = this.reportGridData.groupedItems.map(x => x.timeTotalFor.timeActualTotalFor)
		}

		return Math.max.apply(Math, arr);
	}

	private calcNumberWidth(maxTotalTrackedTime: number, isGroupByUser: boolean) {
		if (isGroupByUser) {
			const totalTrackedTimeArr = this.reportGridData.groupedItems
				.filter(x => (x.groupByType.workingHoursPerDay || 8) === maxTotalTrackedTime)
				.map(x => x.timeTotalFor.timeActualTotalFor);

			maxTotalTrackedTime = Math.max.apply(Math, totalTrackedTimeArr);
		}

		return (this.calcTrackedHours(maxTotalTrackedTime) + 'h').length * 7.5;
	}

	private static getNumberOfWorkingDays(period: DatePeriod): number {
		let day = period.dateFrom.clone();
		let result = 0;

		while (DateUtils.convertMomentToUTC(day) <= DateUtils.convertMomentToUTC(period.dateTo)) {
			if (day.day() !== 0 && day.day() !== 6) {
				result++;
			}
			day.add(1, 'days');
		}

		return result;
	}

	private displayChart(isGroupByUser: boolean): void {
		const maxTotalValue = this.calcMaxTotalValue(isGroupByUser);
		const chartNumberWidth = this.calcNumberWidth(maxTotalValue, isGroupByUser);
		const maxChartWidth = 240 - 61 - chartNumberWidth;
		this.chartWidthParam = maxChartWidth / maxTotalValue;

		this.numberOfWorkingDays = ReportsComponent.getNumberOfWorkingDays(this.dateResponse.datePeriod);
	}

	// FILTERS

	groupByChange(): void {
		this.reportQuery.groupById = this.groupModel.id;
		this.getReportGrid();
	}

	toggleClient(clientIds: number[] = []): void {
		this.selectedClients = [];

		clientIds.forEach((clientId: number) => {
			this.selectedClients.push(this.clients.find((client: ClientDetail) => client.clientId === clientId));
		});

		this.getProjectItems(this.selectedClients.length ? this.selectedClients : this.clients);

		this.reportQuery.projectIds = this.reportQuery.projectIds.filter((projectId: number) => {
			return this.projects.find((project: ProjectDetail) => project.projectId === projectId);
		});

		this.toggleProject(this.reportQuery.projectIds);
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

		this.reportQuery.memberIds = this.reportQuery.memberIds.filter((userId: number) => {
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
