import { Injectable } from '@angular/core';

export const ROWS_ON_PAGE = 15;
export const EMAIL_PATTERN = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()\.,;\s@\"]+\.{0,1})+[^<>()\.,;:\s@\"]{2,})$/;
export const URL_PATTERN = /^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$/i;

@Injectable()
export class ConstantService {
	apiBaseUrl = '/api/v1';
	adminApi = '/api/v1/Admin/';
	profileApi: string = '/api/v1/Profile';
	reportsApi: string = '/api/v1/Reports';
	timeEntriesApi: string = '/api/v1/TimeEntries/';
}
