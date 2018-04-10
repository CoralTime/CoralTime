import { TimeValues } from './calendar';

export interface ReportDropdowns {
	values: ReportDropdownsDetails;
	currentQuery: ReportFilters;
}

export class ReportFilters {
	clientIds: number[];
	dateFrom: string;
	dateStaticId: number;
	dateTo: string;
	groupById: number;
	memberIds: number[];
	projectIds: number[];
	queryId: number;
	queryName: string;
	showColumnIds: number[];

	constructor(obj: any) {
		this.clientIds = obj.clientIds || [];
		this.dateFrom = obj.dateFrom;
		this.dateStaticId = obj.dateStaticId;
		this.dateTo = obj.dateTo;
		this.groupById = obj.groupById || 3;
		this.memberIds = obj.memberIds || [];
		this.projectIds = obj.projectIds || [];
		this.queryId = obj.queryId;
		this.queryName = obj.queryName;
		this.showColumnIds = obj.showColumnIds || [1, 2, 3, 4];
	}
}

export interface ReportDropdownsDetails {
	customQueries: ReportFilters[];
	dateStatic: DateStatic[];
	filters: ClientDetail[];
	groupBy: GroupByItem[];
	showColumns: ShowColumn[];
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

export interface ReportGrid {
	groupedItems: ReportGridView[];
	timeTotal: TimeTotal;
}

export interface TimeTotal {
	timeActualTotal: number;
	timeEstimatedTotal: number;
}

export class ReportGridView {
	groupByType: GroupByType;
	items: ReportItem[];
	timeTotalFor: TimeTotalFor;
}

export interface TimeTotalFor {
	timeActualTotalFor: number;
	timeEstimatedTotalFor: number;
}

export interface GroupByType {
	clientId: number;
	clientName: string;
	date: string;
	memberId: number;
	memberName: string;
	projectId: number;
	projectName: string;
}

export interface ReportItem {
	clientId: number;
	clientName: string;
	date: Date;
	memberId: number;
	memberName: string;
	notes: string;
	taskId: number;
	taskName: string;
	timeValues: TimeValues;
}

export interface DateStatic {
	id: number;
	dateFrom: string;
	dateTo: string;
	description: string;
}

export interface GroupByItem {
	id: number;
	description: string;
}

export interface ShowColumn {
	id: string;
	description: string;
}
