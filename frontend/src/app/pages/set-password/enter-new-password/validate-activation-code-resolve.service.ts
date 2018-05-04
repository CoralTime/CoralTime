import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { SetPasswordService } from './set-password.service';
import { Observable } from 'rxjs';

@Injectable()
export class ValidateRestoreCodeResolve implements Resolve<boolean> {
	constructor(private service: SetPasswordService) {
	}

	resolve(route: ActivatedRouteSnapshot): Observable<boolean> | boolean {
		let restoreCode = route.queryParams['restoreCode'];
		if (!restoreCode) {
			return false;
		}
		return this.service.validateRestoreCode(restoreCode)
			.map((restoreCodeValid: boolean) => {
				return restoreCodeValid;
			});
	}
}
