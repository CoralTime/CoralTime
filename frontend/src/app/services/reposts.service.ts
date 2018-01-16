import { Http, RequestOptions, Response, ResponseContentType } from '@angular/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';
import { saveAs as importedSaveAs } from 'file-saver';
import { AuthService } from '../core/auth/auth.service';
import { SendReportsFormModel } from '../pages/reports/reports-send/reports-send.component';
import { DatePeriod } from '../pages/reports/range-datepicker/range-datepicker.service';
import * as moment from 'moment';

export interface ReportDropdowns {
	currentUserFullName: string;
	currentUserId: number;
	isAdminCurrentUser: boolean;
	isManagerCurrentUser: boolean;
	clientsDetails: ClientDetail[];
}

export interface ClientDetail {
	clientId: number;
	clientName: string;
	isClientActive: boolean;
	projectsDetails: ProjectDetail[];
}

export interface ProjectDetail {
	isProjectActive: boolean;
	isUserManagerOnProject: boolean;
	projectId: number;
	projectName: string;
	roleId: number;
	usersDetails: UserDetail[];
}

export interface UserDetail {
	isUserActive: boolean;
	roleId: number;
	userId: number;
	userFullName: string;
}

export class ReportGrid {
	grandActualTime: number;
	grandEstimatedTime: number;
	reportsGridView: ReportGridView[];
}

export class ReportGridView {
	clientId: number;
	clientName: string;
	date: string;
	items: ReportItem[];
	memberId: number;
	memberName: string;
	projectId: number;
	projectName: string;
	totalActualTime: number;
	totalEstimatedTime: number;
}

export interface ReportItem {
	actualTime: number;
	clientId: number;
	clientName: string;
	date: Date;
	description: string;
	estimatedTime: number;
	fromTime: number;
	memberId: number;
	memberName: string;
	taskId: number;
	taskName: string;
	toTime: number;
}

export interface GroupByItem {
	groupById: number;
	groupByDescription: string;
}

export class ReportsSettings {
	datePeriod: DatePeriod;
	projectIds: number[];
	userIds: number[];
	clientIds: number[];
	groupById: number;
	showColumnIds: number[];

	constructor(data: any) {
		if (!data) {
			return;
		}

		this.datePeriod = data.datePeriod;
		this.projectIds = data.projectIds;
		this.userIds = data.userIds;
		this.clientIds = data.clientIds;
		this.groupById = data.groupById;
		this.showColumnIds = data.showColumnIds;
	}
}

const REPORTS_SETTINGS = 'REPORTS_SETTINGS';

@Injectable()
export class ReportsService {
	private reportsSettings: ReportsSettings;

	constructor(private authService: AuthService,
	            private constantService: ConstantService,
	            private http: Http) {
		if (localStorage.hasOwnProperty(REPORTS_SETTINGS)) {
			this.reportsSettings = JSON.parse(localStorage.getItem(REPORTS_SETTINGS));

			if (this.reportsSettings.datePeriod) {
				this.reportsSettings.datePeriod.dateFrom = moment(this.reportsSettings.datePeriod.dateFrom);
				this.reportsSettings.datePeriod.dateTo = moment(this.reportsSettings.datePeriod.dateTo);
			}
		} else {
			this.setDefaultReportSettings();
		}
		this.authService.onChange.subscribe(() => {
			this.setDefaultReportSettings();
		});
	}

	getReportDropdowns(): Observable<ReportDropdowns> {
		return this.http.get(this.constantService.reportsApi).map((res: Response) => res.json());
	}

	getReportsGroupBy(): Observable<GroupByItem[]> {
		return this.http.get(this.constantService.reportsApi + 'GroupBy').map((res: Response) => res.json());
	}

	getReportGrid(filters: any): Observable<ReportGrid> {
		return this.http.post(this.constantService.reportsApi, filters).map((res: Response) => res.json());
	}

	getReportSettings(): ReportsSettings {
		return this.reportsSettings;
	}

	setReportSettings(reportsSettings: any): void {
		this.reportsSettings = new ReportsSettings(reportsSettings);
		localStorage.setItem(REPORTS_SETTINGS, JSON.stringify(this.reportsSettings));
	}

	setDefaultReportSettings(): void {
		let reportsSettings = {
			datePeriod: null,
			projectIds: [],
			userIds: [],
			clientIds: [],
			groupById: 3,
			showColumnIds: [1, 2, 3, 4]
		};
		this.setReportSettings(reportsSettings);
	}

	exportAs(filters: any): Observable<void> {
		let options = new RequestOptions({responseType: ResponseContentType.Blob});

		return this.http.post(this.constantService.reportsApi + 'Export', filters, options).map(data => {
			let filename = 'reports';
			if (data.headers.has('Content-Disposition')) {
				let cdHeader = data.headers.get('Content-Disposition');
				filename = cdHeader.replace(/(.*filename=")(.*)(";.*)/, '$2');
			}
			importedSaveAs(data.blob(), filename);
		});
	}

	sendReports(filters: SendReportsFormModel): Observable<any> {
		return this.http.post(this.constantService.sendReportsApi, filters);
	}
}
