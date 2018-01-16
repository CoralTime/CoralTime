import { Injectable } from '@angular/core';

export const ROWS_ON_PAGE = 15;
export const EMAIL_PATTERN = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()\.,;\s@\"]+\.{0,1})+[^<>()\.,;:\s@\"]{2,})$/;

@Injectable()
export class ConstantService {
	apiBaseUrl = '/api/v1';
	profileApi: string = '/api/v1/Profile';
	reportsApi: string = '/api/v1/Reports';
	sendReportsApi: string = '/api/v1/SendReports';
	timeEntriesApi: string = '/api/v1/TimeEntries';
	userAvatarApi: string = '/api/v1/Profile/Avatar/';
	userIconApi: string = '/api/v1/Profile/Icon/';
}