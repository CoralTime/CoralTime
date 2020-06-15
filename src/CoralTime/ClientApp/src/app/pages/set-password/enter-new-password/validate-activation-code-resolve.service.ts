import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { SetPasswordService } from './set-password.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';

@Injectable()
export class ValidateRestoreCodeResolve implements Resolve<boolean> {
	constructor(private loadingService: LoadingMaskService,
	            private service: SetPasswordService) {
	}

	resolve(route: ActivatedRouteSnapshot): Observable<boolean> | boolean {
		let restoreCode = route.queryParams['restoreCode'];
		if (!restoreCode) {
			return false;
		}

		this.loadingService.addLoading();
		return this.service.validateRestoreCode(restoreCode)
			.map((restoreCodeValid: boolean) => {
				this.loadingService.removeLoading();
				return restoreCodeValid;
			});
	}
}
