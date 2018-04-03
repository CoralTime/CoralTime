import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Subscriber } from 'rxjs/Subscriber';
import { AuthService } from './auth/auth.service';
import { NotificationService } from 'app/core/notification.service';

interface CallerRequest {
	failedRequest: HttpRequest<any>;
	subscriber: Subscriber<any>;
}

@Injectable()
export class RefreshTokenInterceptor implements HttpInterceptor {
	private authService: AuthService;
	private http: HttpClient;
	private refreshInProgress: boolean;
	private requests: CallerRequest[] = [];

	constructor(private injector: Injector,
	            private notificationService: NotificationService) {
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
						switch (err.status) {
							case 401 :
								if (!this.isTokenExpired(err)) {
									this.navigateToLogin();
									return Observable.throw(err);
								}

								this.handleUnauthorizedError(subscriber, req);
								break;
							case 403 :
								this.notificationService.danger('You don\'t have permission for this action');
								break;
							default:
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

	private handleUnauthorizedError(subscriber: Subscriber<any>, request: HttpRequest<any>): void {
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
						this.navigateToLogin();
					});
		}
	}

	private repeatFailedRequests(): void {
		this.requests.forEach((c) => {
			const requestWithNewToken = c.failedRequest.clone({
				headers: c.failedRequest.headers.set('Authorization', 'Bearer ' + this.authService.getAuthUser().accessToken)
			});

			this.repeatRequest(requestWithNewToken, c.subscriber);
		});

		this.requests = [];
	}

	private repeatRequest(requestWithNewToken: HttpRequest<any>, subscriber: Subscriber<any>): void {
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
		return error.headers.has('www-authenticate') && /expired/.test(error.headers.get('www-authenticate'));
	}

	private navigateToLogin(): void {
		if (this.authService.isLoggedIn()) {
			this.notificationService.danger('Your session is expired.');
			this.authService.logout();
		}
	}
}
