import { MdDialog } from '@angular/material';
import { Router } from '@angular/router';
import { Http, Headers, Response } from '@angular/http';
import { Injectable, EventEmitter } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { AuthUser } from './auth-user';
import { ImpersonationService } from '../../services/impersonation.service';
import { UserInfoService } from './user-info.service';

const AUTH_USER_STORAGE_KEY = 'APPLICATION_USER';

@Injectable()
export class AuthService {
	private readonly clientId: string = 'coraltimeapp';
	private readonly clientIdSSO: string = 'coraltimeazure';
	private readonly clientSecret: string = 'secret';
	private readonly scope: string = 'WebAPI offline_access openid profile roles';
	private authUser: AuthUser;
	private _isUserAdminOrManager: boolean = false;
	private refreshTokenObservable: Observable<Response>;

	public adminOrManagerParameterOnChange: EventEmitter<void> = new EventEmitter<void>();
	public onChange: EventEmitter<AuthUser> = new EventEmitter<AuthUser>();

	get isUserAdminOrManager(): boolean {
		return this._isUserAdminOrManager;
	}

	set isUserAdminOrManager(adminOrManagerParameter: boolean) {
		this._isUserAdminOrManager = adminOrManagerParameter;
		this.adminOrManagerParameterOnChange.emit();
	}

	constructor(private http: Http,
	            private impersonateService: ImpersonationService,
	            private mdDialog: MdDialog,
	            private router: Router,
	            private userInfoService: UserInfoService) {
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
		});
	}

	refreshToken(): Observable<Response> {
		if (this.refreshTokenObservable) {
			return this.refreshTokenObservable;
		}

		let headers = new Headers();
		headers.append('Content-Type', 'application/x-www-form-urlencoded');

		let params = new URLSearchParams();

		if (localStorage.hasOwnProperty(AUTH_USER_STORAGE_KEY)) {
			this.authUser = JSON.parse(localStorage.getItem(AUTH_USER_STORAGE_KEY));
			let headerClientId = this.authUser.isSso ? this.clientIdSSO : this.clientId;
			params.append('client_id', headerClientId);
			params.append('client_secret', this.clientSecret);
			params.append('grant_type', 'refresh_token');
			params.append('refresh_token', this.authUser.refreshToken);
			params.append('scope', this.scope);

			let body = params.toString();

			this.refreshTokenObservable = this.http.post('/connect/token', body, {
				headers: headers
			})
				.flatMap((response: Response) => {
					this.setAuthUser(new AuthUser(response.json(), this.authUser.isSso));
					this.refreshTokenObservable = null;
					return Observable.of(response);
				}).catch(error => {
					this.refreshTokenObservable = null;
					this.logout();
					return Observable.throw(error);
				})
				.share();

			return this.refreshTokenObservable;
		} else {
			this.logout();
			return this.refreshTokenObservable;
		}
	}

	logout(ignoreRedirect?: boolean): void {
		this.impersonateService.stopImpersonation(true);
		this.authUser = null;
		this.userInfoService.setUserInfo(null);
		this.mdDialog.closeAll();
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