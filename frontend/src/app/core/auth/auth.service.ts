import { MatDialog } from '@angular/material';
import { Router } from '@angular/router';
import { Http, Headers, Response } from '@angular/http';
import { Injectable, EventEmitter } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { AuthUser } from './auth-user';
import { ImpersonationService } from '../../services/impersonation.service';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

const AUTH_USER_STORAGE_KEY = 'APPLICATION_USER';

@Injectable()
export class AuthService {
	private readonly clientId: string = 'coraltimeapp';
	private readonly clientIdSSO: string = 'coraltimeazure';
	private readonly clientSecret: string = 'secret';
	private readonly scope: string = 'WebAPI offline_access openid profile roles';
	private authUser: AuthUser;
	private _isUserAdminOrManager: boolean = false;

	public adminOrManagerParameterOnChange: EventEmitter<void> = new EventEmitter<void>();
	public onChange: EventEmitter<AuthUser> = new EventEmitter<AuthUser>();

	private isRefreshingToken: boolean = false;
	private tokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

	get isUserAdminOrManager(): boolean {
		return this._isUserAdminOrManager;
	}

	set isUserAdminOrManager(adminOrManagerParameter: boolean) {
		this._isUserAdminOrManager = adminOrManagerParameter;
		this.adminOrManagerParameterOnChange.emit();
	}

	constructor(private http: Http,
	            private impersonateService: ImpersonationService,
	            private matDialog: MatDialog,
	            private router: Router) {
		if (localStorage.hasOwnProperty(AUTH_USER_STORAGE_KEY)) {
			this.authUser = JSON.parse(localStorage.getItem(AUTH_USER_STORAGE_KEY));
		}
	}

	login(username: string, password: string): Observable<boolean> {
		let headers = new Headers();
		headers.append('Content-Type', 'application/x-www-form-urlencoded');

		let params = new URLSearchParams();
		params.append('client_id', this.clientId);
		params.append('client_secret', this.clientSecret);
		params.append('grant_type', 'password');
		params.append('username', username);
		params.append('password', password);
		params.append('scope', this.scope);

		let body = params.toString();

		return this.http.post('/connect/token', body, {
			headers: headers
		}).map(response => {
			this.setAuthUser(new AuthUser(response.json(), false));
			return true;
		});
	}

	loginSSO(id_token: string): Observable<boolean> {
		let headers = new Headers();
		headers.append('Content-Type', 'application/x-www-form-urlencoded');
		let params = new URLSearchParams();
		params.append('grant_type', 'azureAuth');
		params.append('id_token', id_token);
		params.append('scope', this.scope);
		params.append('client_id', this.clientIdSSO);
		params.append('client_secret', this.clientSecret);
		let body = params.toString();
		return this.http.post('/connect/token', body, {
			headers: headers
		}).map(response => {
			this.setAuthUser(new AuthUser(response.json(), true));
			return true;
		}).catch(() => this.router.navigate(['/error']));
	}

	refreshToken(): Observable<Response> {
		if (!localStorage.hasOwnProperty(AUTH_USER_STORAGE_KEY)) {
			this.logout();
			return null;
		}

		if (!this.isRefreshingToken) {
			let headers = new Headers();
			headers.append('Content-Type', 'application/x-www-form-urlencoded');

			let params = new URLSearchParams();
			this.isRefreshingToken = true;
			this.tokenSubject.next(null);

			this.authUser = JSON.parse(localStorage.getItem(AUTH_USER_STORAGE_KEY));
			let headerClientId = this.authUser.isSso ? this.clientIdSSO : this.clientId;
			params.append('client_id', headerClientId);
			params.append('client_secret', this.clientSecret);
			params.append('grant_type', 'refresh_token');
			params.append('refresh_token', this.authUser.refreshToken);
			params.append('scope', this.scope);

			let body = params.toString();
			return this.http.post('/connect/token', body, {headers: headers})
				.flatMap((response: Response) => {
					if (response) {
						this.setAuthUser(new AuthUser(response.json(), this.authUser.isSso));
						this.tokenSubject.next(response);
						return Observable.of(response);
					}

					this.logout();
					return Observable.of(null);
				})
				.catch(error => {
					this.logout();
					return Observable.throw(error);
				})
				.finally(() => {
					this.isRefreshingToken = false;
				});
		} else {
			return this.tokenSubject
				.filter(token => token != null)
				.take(1)
				.switchMap(response => {
					return Observable.of(response);
				});
		}
	}

	logout(ignoreRedirect?: boolean): void {
		this.impersonateService.stopImpersonation(true);
		this.authUser = null;
		this.matDialog.closeAll();
		localStorage.removeItem(AUTH_USER_STORAGE_KEY);
		if (!ignoreRedirect) {
			this.router.navigate(['/login']);
		}
		this.onChange.emit(null);
	}

	private setAuthUser(authUser: AuthUser): void {
		this.authUser = authUser;
		localStorage.setItem(AUTH_USER_STORAGE_KEY, JSON.stringify(this.authUser));
		this.onChange.emit(this.authUser);
	}

	getAuthUser(): AuthUser {
		return this.authUser;
	}

	isLoggedIn(): boolean {
		return !!this.getAuthUser();
	}
}
