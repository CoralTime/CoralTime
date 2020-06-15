import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginService, LoginSettings } from './login.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Injectable()
export class LoginResolve implements Resolve<any> {
	constructor(private loadingService: LoadingMaskService,
	            private service: LoginService) {
	}

	resolve(route: ActivatedRouteSnapshot): Observable<LoginSettings> {
		this.loadingService.addLoading();
		return this.service.getAuthenticationSettings()
			.map((loginSettings: LoginSettings) => {
				this.loadingService.removeLoading();
				return loginSettings;
			});
	}
}
