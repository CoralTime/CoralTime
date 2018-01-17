import * as Raven from 'raven-js';
import { ErrorHandler } from '@angular/core';
import { environment } from '../../../environments/environment';

Raven.config('https://2c2d7c3d3cfa405094e51a533a44a5a4@sentry.io/163165')
	.install();

export class RavenErrorHandler implements ErrorHandler {
	handleError(err: any): void {
		if (environment.production) {
			Raven.captureException(err.originalError);
		} else {
			throw err;
		}
	}
}
