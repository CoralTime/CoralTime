import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Rx';
import { AuthService } from './auth/auth.service';
import { LoadingIndicatorService } from './loading-indicator.service';

@Injectable()
export class ApplyTokenInterceptor implements HttpInterceptor {
	private http: HttpClient;
	private authService: AuthService;
	private loadingService: LoadingIndicatorService;

	constructor(private injector: Injector) {
		console.log('ApplyTokenInterceptor created');
	}

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		if (!this.http) {
			this.http = this.injector.get(HttpClient);
		}
		if (!this.authService) {
			this.authService = this.injector.get(AuthService);
		}
		if (!this.loadingService) {
			this.loadingService = this.injector.get(LoadingIndicatorService);
		}

		if (!req.url.includes('api/')) {
			return next.handle(req);
		}

		const authReq = req.clone({
			headers: req.headers.set('Authorization', 'Bearer ' + this.authService.getAuthUser().accessToken)
		});

		console.log('Sending request with new header now ...', authReq);

		this.loadingService.start();
		return next.handle(authReq).finally(() => {
			this.loadingService.complete();
		})
	}
}
