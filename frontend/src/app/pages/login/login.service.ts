import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export class LoginSettings {
	enableAzure: boolean;
	azureSettings: AzureSettings;
}

export class AzureSettings {
	tenant: string;
	clientId: string;
	redirectUrl: string;
}

@Injectable()
export class LoginService {
	constructor(private http: HttpClient) {
	}

	getAuthenticationSettings(): Observable<LoginSettings> {
		let url = '/api/v1/AuthenticationSettings';
		return this.http.get<LoginSettings>(url);
	}
}
