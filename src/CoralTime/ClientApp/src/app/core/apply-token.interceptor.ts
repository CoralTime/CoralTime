import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Rx';
import { AuthService } from './auth/auth.service';
import { ImpersonationService } from '../services/impersonation.service';

@Injectable()
export class ApplyTokenInterceptor implements HttpInterceptor {
	private authService: AuthService;
	private http: HttpClient;

	constructor(private impersonationService: ImpersonationService,
	            private injector: Injector) {
	}

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		if (!this.http) {
			this.http = this.injector.get(HttpClient);
		}
		if (!this.authService) {
			this.authService = this.injector.get(AuthService);
		}
		if (!req.url.includes('api/')) {
			return next.handle(req);
		}

		const authReq = req.clone({
			headers: this.extendHeaders(req.headers)
		});

		return next.handle(authReq)
	}

	private extendHeaders(headers: HttpHeaders): HttpHeaders {
		if (!headers.has('Authorization') && this.authService.isLoggedIn()) {
			let authUser = this.authService.authUser;
			if (authUser && authUser.accessToken) {
				headers = headers.set('Authorization', 'Bearer ' + authUser.accessToken);
			}
		}

		headers = this.setDefaultHeaders(headers);
		headers = this.impersonate(headers);
		return headers;
	}

	private impersonate(headers: HttpHeaders): HttpHeaders {
		if (!headers.has('Impersonate') && this.impersonationService.impersonationMember) {
			headers = headers.set('Impersonate', this.impersonationService.impersonationMember);
		}
		if (headers.has('Impersonate') && !this.impersonationService.impersonationMember) {
			headers = headers.delete('Impersonate');
		}

		return headers;
	}

	private setDefaultHeaders(headers: HttpHeaders): HttpHeaders {
		if (!headers.has('Cache-control')) {
			headers = headers.append('Cache-control', 'no-cache,no-store');
		}
		if (!headers.has('Expires')) {
			headers = headers.append('Expires', '0');
		}
		if (!headers.has('Pragma')) {
			headers = headers.append('Pragma', 'no-cache');
		}

		return headers;
	}
}
