import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ODataConfiguration } from './config';
import { ODataService } from './odata';
import { NotificationService } from '../../core/notification.service';

@Injectable()
export class ODataServiceFactory {
	constructor(private config: ODataConfiguration,
	            private http: HttpClient,
	            private notificationService: NotificationService) {
	}

	CreateService<T>(typeName: string): ODataService<T> {
		return new ODataService<T>(typeName, this.http, this.config, this.notificationService);
	}

	CreateServiceWithOptions<T>(typeName: string, config: ODataConfiguration): ODataService<T> {
		return new ODataService<T>(typeName, this.http, config, this.notificationService);
	}
}
