import { Http, RequestOptions, Response, ResponseContentType } from '@angular/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';
import { saveAs as importedSaveAs } from 'file-saver';
import { SendReportsFormModel } from '../pages/reports/reports-send/reports-send.component';
import { DatePeriod } from '../pages/reports/range-datepicker/range-datepicker.service';
import { CustomSelectItem } from '../shared/form/multiselect/multiselect.component';

export interface ReportDropdowns {
	values: ReportDropdownsDetails;
	valuesSaved: ReportFilters;
}

export class ReportFilters {
	clientIds: number[];
	dateFrom: Date | string;
	dateTo: Date | string;
	groupById: number;
	memberIds: number[];
	projectIds: number[];
	showColumnIds: number[];

	constructor(obj: any) {
		this.clientIds = obj.clientIds || [];
		this.dateFrom = obj.dateFrom;
		this.dateTo = obj.dateTo;
		this.groupById = obj.groupById;
		this.memberIds = obj.memberIds || [];
		this.projectIds = obj.projectIds || [];
		this.showColumnIds = obj.showColumnIds || [];
	}
}

export interface ReportDropdownsDetails {
	filters: ClientDetail[];
	groupBy: GroupByItem[];
	showColumns: CustomSelectItem;
	userDetails: CurrentUserDetails;
}

export interface CurrentUserDetails {
	currentUserFullName: string;
	currentUserId: number;
	isAdminCurrentUser: boolean;
	isManagerCurrentUser: boolean;
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
	id: number;
	description: string;
}

@Injectable()
export class ReportsService {
	constructor(private constantService: ConstantService,
	            private http: Http) {
	}

	getReportDropdowns(): Observable<ReportDropdowns> {
		return this.http.get(this.constantService.reportsApi).map((res: Response) => res.json());
	}

	getReportGrid(filters: any): Observable<ReportGrid> {
		return this.http.post(this.constantService.reportsApi, filters).map((res: Response) => res.json());
	}

	exportAs(filters: any): Observable<void> {
		let options = new RequestOptions({responseType: ResponseContentType.Blob});

		return this.http.post(this.constantService.reportsApi + 'ExportFile', filters, options).map(data => {
			let filename = 'reports';
			if (data.headers.has('Content-Disposition')) {
				let cdHeader = data.headers.get('Content-Disposition');
				filename = cdHeader.replace(/(.*filename=")(.*)(";.*)/, '$2');
			}
			importedSaveAs(data.blob(), filename);
		});
	}

	sendReports(filters: SendReportsFormModel): Observable<any> {
		return this.http.post(this.constantService.reportsApi + 'ExportEmail', filters);
	}
}
