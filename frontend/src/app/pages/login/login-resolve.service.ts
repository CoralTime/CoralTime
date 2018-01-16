import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginService, LoginSettings } from './login.service';

@Injectable()
export class LoginResolve implements Resolve<any> {
	constructor(private service: LoginService) {
	}

	resolve(route: ActivatedRouteSnapshot): Observable<LoginSettings> {
		return this.service.getAuthenticationSettings()
			.map((loginSettings: LoginSettings) => {
				return loginSettings;
			});
	}
}
