
import {of as observableOf,  Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class SettingsService {
	getDefaultProjectRoleName(): Observable<string> {
		return observableOf('team member');
	}

	getDefaultDateFormat(): number {
		return 0;
	}

	getDefaultTimeFormat(): number {
		return 0;
	}

	getIsEstimatedTimeEnabled(): boolean {
		return true;
	}
}
