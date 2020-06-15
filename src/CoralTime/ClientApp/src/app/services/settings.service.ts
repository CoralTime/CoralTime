
import {of as observableOf,  Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class SettingsService {
	getDefaultProjectRoleName(): Observable<string> {
		return observableOf('team member');
	}

	getIsEstimatedTimeEnabled(): boolean {
		return true;
	}
}
