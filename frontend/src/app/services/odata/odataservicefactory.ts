import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { ODataService } from './odata';
import { ODataConfiguration } from './config';
import { NotificationService } from '../../core/notification.service';

@Injectable()
export class ODataServiceFactory {
	constructor(private http: Http,
	            private config: ODataConfiguration,
	            private notificationService: NotificationService) {
	}

	CreateService<T>(typeName: string, handleError?: (err: any) => any): ODataService<T> {
		return new ODataService<T>(typeName, this.http, this.config, this.notificationService);
	}

	CreateServiceWithOptions<T>(typeName: string, config: ODataConfiguration): ODataService<T> {
		return new ODataService<T>(typeName, this.http, config, this.notificationService);
	}
}
