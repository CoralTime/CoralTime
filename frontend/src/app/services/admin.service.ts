import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ODataConfiguration } from './odata/config';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AdminService {
	constructor(protected config: ODataConfiguration,
	            private http: HttpClient) {
	}

	resetCache(): Observable<any> {
		let url = '/api/v1/Admin/ResetCache/';
		return this.http.get(url);
	};

	updateManagersRoles(): Observable<any> {
		let url = '/api/v1/Admin/UpdateManagerRoles/';
		return this.http.get(url);
	};

	updateClaims(): Observable<any> {
		let url = '/api/v1/Admin/UpdateClaims/';
		return this.http.get(url);
	};

	updateAvatars(): Observable<any> {
		let url = '/api/v1/Admin/SaveImagesFromDbToStaticFiles/';
		return this.http.get(url);
	};

	sendNotificationFromProject(): Observable<any> {
		let url = '/api/v1/Admin/NotificationsByProjectSettings/';
		return this.http.get(url);
	};

	sendNotificationsWeekly(): Observable<any> {
		let url = '/api/v1/Admin/NotificationsByProjectSettings/';
			return this.http.get(url);
	};
}
