import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class SettingsService {
	getDefaultProjectRoleName(): Observable<string> {
		return Observable.of('team member');
	}
}
