import { ErrorHandler, Injector  } from '@angular/core';
import { AppInsightsService } from '@markpieszak/ng-application-insights';
import { environment } from '../../environments/environment';

export class CustomErrorHandler implements ErrorHandler {
     constructor(private injector: Injector) {
     }
    
	handleError(err: any): void {
	    const appInsightsService = this.injector.get(AppInsightsService);
    	appInsightsService.trackException(err);

		if (!environment.production) {
			throw err;
		}
	}
}
