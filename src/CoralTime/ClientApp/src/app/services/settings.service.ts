
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

	getDefaultProjectIsPrivate(): boolean {
		return true;
	}

	getIsEstimatedTimeEnabled(): boolean {
		return true;
	}

	getIsFromToRequired(): boolean {
		return false;
	}

	getTimeEntryMinutesIncrement(): number {
		return 5;
	}

	getIsTimerEnabled(): boolean {
		return true;
	}
}
