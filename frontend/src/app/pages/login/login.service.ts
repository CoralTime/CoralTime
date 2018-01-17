import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';

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
	constructor(private http: Http) {
	}

	getAuthenticationSettings(): Observable<LoginSettings> {
		let url = '/api/v1/AuthenticationSettings';
		return this.http.get(url)
			.map(res => {
				return res.json();
			});
	}
}
