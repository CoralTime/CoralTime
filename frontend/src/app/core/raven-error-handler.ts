import { ErrorHandler } from '@angular/core';
import { environment } from '../../environments/environment';

export class CustomErrorHandler implements ErrorHandler {
	handleError(err: any): void {
		if (!environment.production) {
			throw err;
		}
	}
}
