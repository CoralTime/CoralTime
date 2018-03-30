import { Injectable } from '@angular/core';
import { ODataService } from './odata';
import { ODataConfiguration } from './config';
import { NotificationService } from '../../core/notification.service';
import { CustomHttp } from '../../core/custom-http';

@Injectable()
export class ODataServiceFactory {
	constructor(private http: CustomHttp,
	            private config: ODataConfiguration,
	            private notificationService: NotificationService) {
	}

	CreateService<T>(typeName: string): ODataService<T> {
		return new ODataService<T>(typeName, this.http, this.config, this.notificationService);
	}

	CreateServiceWithOptions<T>(typeName: string, config: ODataConfiguration): ODataService<T> {
		return new ODataService<T>(typeName, this.http, config, this.notificationService);
	}
}
