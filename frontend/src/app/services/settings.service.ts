import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { ODataServiceFactory, ODataService } from './odata';
import { Setting } from '../models/setting';

@Injectable()
export class SettingsService {
	readonly odata: ODataService<Setting>;

	constructor(private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<Setting>('Settings');
	}

	getDefaultProjectRoleName(): Observable<string> {
		return Observable.of('team member');
	}
}