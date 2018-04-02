import { Injectable, Injector } from '@angular/core';
import { Request, XHRBackend, RequestOptions, Response, Http, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { AuthService } from './auth/auth.service';
import { NotificationService } from './notification.service';
import { ImpersonationService } from '../services/impersonation.service';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

interface CustomRequest {
	url: string | Request;
	options: RequestOptionsArgs;
};

@Injectable()
export class CustomHttp extends Http {
	private refreshInProgress: boolean;
	private tokenSubject: BehaviorSubject<CustomRequest> = new BehaviorSubject<CustomRequest>(null);

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
		if (error.status === 401) {
			if (!this.isTokenExpired(error)) {
				this.navigateToLogin();
				return Observable.throw(error);
			}

			if (!this.refreshInProgress) {
				console.log(2);
				this.refreshInProgress = true;
				this.tokenSubject.next(null);
				let authService = this.injector.get(AuthService);
				return authService.refreshToken()
					.flatMap((res) => {
						this.removeExtendedHeaders(url, options);
						if (url instanceof Request) {
							url.headers = this.extendHeaders(url.headers);
						} else {
							options.headers = this.extendHeaders(options.headers);
						}

						this.tokenSubject.next({url, options});
						// console.log('1 repeat', url, (<Request>url).headers.get('Authorization'));
						return this.repeatRequest(url, options);
					})
					.catch(err => {
						this.navigateToLogin();
						this.tokenSubject.next(err);

						return Observable.throw(err);
					})
					.finally(() => {
						this.refreshInProgress = false;
					});
			} else {
				return this.tokenSubject
					.filter(res => res !== null)
					.take(1)
					.switchMap(res => {
							// console.log('2 repeat', (<Request>url).headers.get('Authorization'));
							return this.repeatRequest(url, options)}
						);
			}
		}

		if (error.status === 403) {
			this.notificationService.danger('You don\'t have permission for this action');
		}

		console.log(66);
		return Observable.throw(error);
	}

	private repeatRequest(url: string | Request, options: RequestOptionsArgs = {}): Observable<Response> {
		// if (url instanceof Request) {
		// 	url.headers = this.extendHeaders(url.headers);
		// } else {
		// 	options.headers = this.extendHeaders(options.headers);
		// }

		return super.request(url, options).catch(error => {
			if (error.status === 401) {
				this.navigateToLogin();
			}

			return Observable.throw(error);
		});
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
