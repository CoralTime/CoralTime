import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable} from 'rxjs/Observable';

import { AppInsightsService } from '@markpieszak/ng-application-insights';

@Injectable()
export class AppInsightsInterceptor implements HttpInterceptor {

    constructor(private appInsightsService: AppInsightsService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.appInsightsService.trackEvent(req.url, req.body);
        return next.handle(req);
    }
}