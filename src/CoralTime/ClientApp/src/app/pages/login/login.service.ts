
import {of as observableOf,  Observable } from 'rxjs';

import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface LoginSettings {
	enableAzure: boolean;
	azureSettings: AzureSettings;
	instrumentationKey: string;
	roles: object;
}

export interface AzureSettings {
	tenant: string;
	clientId: string;
	redirectUrl: string;
}

@Injectable()
export class LoginService {
	private authenticationSettings: LoginSettings;

	constructor(private http: HttpClient) {
	}

	getAuthenticationSettings(): Observable<LoginSettings> {
		if (this.authenticationSettings) {
			return observableOf(this.authenticationSettings)
		}

		return this.loadAuthenticationSettings();
	}

	private loadAuthenticationSettings(): Observable<LoginSettings> {
		let url = '/api/v1/AuthenticationSettings';
		return this.http.get<LoginSettings>(url).pipe(
			map((settings: LoginSettings) => this.authenticationSettings = settings));
	}
}
