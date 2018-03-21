import { Injectable, Injector } from '@angular/core';
import { Request, XHRBackend, RequestOptions, Response, Http, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { AuthService } from './auth/auth.service';
import { NotificationService } from './notification.service';
import { ImpersonationService } from '../services/impersonation.service';

@Injectable()
export class CustomHttp extends Http {
	constructor(backend: XHRBackend,
	            defaultOptions: RequestOptions,
	            private injector: Injector,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService) {
		super(backend, defaultOptions);

		// Prevent Ajax Request Caching for Internet Explorer
		defaultOptions.headers.append('Cache-control', 'no-cache');
		defaultOptions.headers.append('Cache-control', 'no-store');
		defaultOptions.headers.append('Pragma', 'no-cache');
		defaultOptions.headers.append('Expires', '0');
	}

	request(url: string | Request, options: RequestOptionsArgs = {}): Observable<Response> {
		if (url instanceof Request) {
			url.headers = this.extendHeaders(url.headers);
		} else {
			options.headers = this.extendHeaders(options.headers);
		}

		return super.request(url, options).catch(error => this.handleError(error, url, options));
	}

	private handleError(error: Response | any, url: string | Request, options: RequestOptionsArgs = {}) {
		// @TODO: send an errorLog entry to the server
		if (error.status === 401) {
			if (this.isTokenExpired(error)) {
				let authService = this.injector.get(AuthService);
				return authService.refreshToken()
					.flatMap(() => {
						this.removeExtendedHeaders(url, options);

						return this.request(url, options);
					})
					.catch((error) => {
						this.navigateToLogin();
						return Observable.throw(error);
					});
			}

			this.navigateToLogin();
		}

		if (error.status === 403) {
			this.notificationService.danger('You don\'t have permission for this action');
		}

		return Observable.throw(error);
	}

	private isTokenExpired(error: Response | any): boolean {
		if (error.headers.has('www-authenticate') && /expired/.test(error.headers.get('www-authenticate'))) {
			return true;
		}
		return false;
	}

	private removeExtendedHeaders(url: string | Request, options: RequestOptionsArgs = {}): void {
		let headers = url instanceof Request ? url.headers : options.headers;
		headers.delete('Authorization');
	}

	private extendHeaders(headers?: Headers): Headers {
		if (!headers) {
			headers = new Headers();
		}

		if (!headers.has('Authorization')) {
			let authService = this.injector.get(AuthService);
			let authUser = authService.getAuthUser();
			if (authUser && authUser.accessToken) {
				headers.set('Authorization', 'Bearer ' + authUser.accessToken);
			}
		}

		headers = this.impersonate(headers);
		return headers;
	}

	private impersonate(headers: Headers): Headers {
		if (!headers.has('Impersonate') && this.impersonationService.impersonationMember) {
			headers.set('Impersonate', this.impersonationService.impersonationMember);
		}
		if (headers.has('Impersonate') && !this.impersonationService.impersonationMember) {
			headers.delete('Impersonate');
		}
		return headers;
	}

	private navigateToLogin(): void {
		let authService = this.injector.get(AuthService);
		if (authService.isLoggedIn()) {
			this.notificationService.danger('Your session is expired.');
			authService.logout();
		}
	}
}
