import { HttpClient, HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Injectable, Injector } from '@angular/core';
import { Subscriber } from 'rxjs/Subscriber';
import { AuthService } from './auth/auth.service';
import { ImpersonationService } from '../services/impersonation.service';
import { NotificationService } from 'app/core/notification.service';
import { RequestOptionsArgs } from '@angular/http';

interface CallerRequest {
	subscriber: Subscriber<any>;
	failedRequest: HttpRequest<any>;
}

@Injectable()
export class RefreshTokenInterceptor implements HttpInterceptor {
	private authService: AuthService;
	private http: HttpClient;
	private refreshInProgress: boolean;
	private requests: CallerRequest[] = [];

	constructor(private injector: Injector,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService) {
		console.log('RefreshTokenInterceptor created');
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

		let observable = new Observable<HttpEvent<any>>((subscriber) => {
			let originalRequestSubscription = next.handle(req)
				.subscribe((response) => {
						subscriber.next(response);
					},
					(err) => {
						if (err.status === 401) {
							this.handleUnauthorizedError(subscriber, req);
						} else {
							subscriber.error(err);
						}
					},
					() => {
						subscriber.complete();
					});

			return () => {
				originalRequestSubscription.unsubscribe();
			};
		});

		return observable;
	}

	private handleUnauthorizedError(subscriber: Subscriber<any>, request: HttpRequest<any>) {
		this.requests.push({subscriber, failedRequest: request});
		if (!this.refreshInProgress) {
			this.refreshInProgress = true;
			this.authService.refreshToken()
				.finally(() => {
					this.refreshInProgress = false;
				})
				.subscribe((authHeader) =>
						this.repeatFailedRequests(),
					() => {
						this.authService.logout();
					});
		}
	}

	private repeatFailedRequests() {
		this.requests.forEach((c) => {
			const requestWithNewToken = c.failedRequest.clone({
				headers: c.failedRequest.headers.set('Authorization', 'Bearer ' + this.authService.getAuthUser().accessToken)
			});

			this.repeatRequest(requestWithNewToken, c.subscriber);
		});
		this.requests = [];
	}

	private repeatRequest(requestWithNewToken: HttpRequest<any>, subscriber: Subscriber<any>) {
		this.http.request(requestWithNewToken).subscribe((res) => {
				subscriber.next(res);
			},
			(err) => {
				if (err.status === 401) {
					this.authService.logout();
				}
				subscriber.error(err);
			},
			() => {
				subscriber.complete();
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

	private extendHeaders(headers?: HttpHeaders): HttpHeaders {
		if (!headers.has('Authorization')) {
			let authUser = this.authService.getAuthUser();
			if (authUser && authUser.accessToken) {
				headers.set('Authorization', 'Bearer ' + authUser.accessToken);
			}
		}

		// headers = this.impersonate(headers);
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
		if (this.authService.isLoggedIn()) {
			this.notificationService.danger('Your session is expired.');
			this.authService.logout();
		}
	}
}
