import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginSettings {
	enableAzure: boolean;
	azureSettings: AzureSettings;
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
			return Observable.of(this.authenticationSettings)
		}

		return this.loadAuthenticationSettings();
	}

	private loadAuthenticationSettings(): Observable<LoginSettings> {
		let url = '/api/v1/AuthenticationSettings';
		return this.http.get<LoginSettings>(url)
			.map((settings: LoginSettings) => this.authenticationSettings = settings);
	}
}
