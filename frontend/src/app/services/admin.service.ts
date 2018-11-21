import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ConstantService } from '../core/constant.service';

@Injectable()
export class AdminService {
	constructor(private constantService: ConstantService,
	            private http: HttpClient) {
	}

	resetCache(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'ResetCache');
	};

	updateManagersRoles(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'UpdateManagerRoles');
	};

	updateClaims(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'UpdateClaims');
	};

	updateAvatars(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'SaveImagesFromDbToStaticFiles');
	};

	sendNotificationFromProject(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'NotificationsByProjectSettings');
	};

	sendNotificationsWeekly(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'NotificationsByProjectSettings');
	};
}
