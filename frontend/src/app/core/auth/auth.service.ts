import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AuthUser } from './auth-user';
import { ImpersonationService } from '../../services/impersonation.service';

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

	get isUserAdminOrManager(): boolean {
		return this._isUserAdminOrManager;
	}

	set isUserAdminOrManager(adminOrManagerParameter: boolean) {
		this._isUserAdminOrManager = adminOrManagerParameter;
		this.adminOrManagerParameterOnChange.emit();
	}

	constructor(private http: HttpClient,
	            private impersonateService: ImpersonationService,
	            private matDialog: MatDialog,
	            private router: Router) {
		if (localStorage.hasOwnProperty(AUTH_USER_STORAGE_KEY)) {
			this.authUser = JSON.parse(localStorage.getItem(AUTH_USER_STORAGE_KEY));

			if (this.isRefreshTokenExpired()) {
				this.logout();
			}
		}
	}

	login(username: string, password: string): Observable<boolean> {
		let headers = {
			'Content-Type': 'application/x-www-form-urlencoded'
		};
		let params = {
			'client_id': this.clientId,
			'client_secret': this.clientSecret,
			'grant_type': 'password',
			'username': username,
			'password': password,
			'scope': this.scope
		};
		let body = this.objectToString(params);

		return this.http.post('/connect/token', body, {headers: headers})
			.map(response => {
				this.setAuthUser(new AuthUser(response, false));
				return true;
			});
	}

	loginSSO(id_token: string): Observable<boolean> {
		let headers = {
			'Content-Type': 'application/x-www-form-urlencoded'
		};
		let params = {
			'client_id': this.clientIdSSO,
			'client_secret': this.clientSecret,
			'id_token': id_token,
			'grant_type': 'azureAuth',
			'scope': this.scope
		};
		let body = this.objectToString(params);

		return this.http.post('/connect/token', body, {headers: headers})
			.map(response => {
				this.setAuthUser(new AuthUser(response, true));
				return true;
			}).catch(() => this.router.navigate(['/error']));
	}

	refreshToken(): Observable<Object> {
		if (!localStorage.hasOwnProperty(AUTH_USER_STORAGE_KEY)) {
			return Observable.throw(new Error('User data not found.'));
		}

		let headers = {
			'Content-Type': 'application/x-www-form-urlencoded'
		};
		this.authUser = JSON.parse(localStorage.getItem(AUTH_USER_STORAGE_KEY));
		let headerClientId = this.authUser.isSso ? this.clientIdSSO : this.clientId;
		let params = {
			'client_id': headerClientId,
			'client_secret': this.clientSecret,
			'grant_type': 'refresh_token',
			'refresh_token': this.authUser.refreshToken,
			'scope': this.scope
		};
		let body = this.objectToString(params);

		return this.http.post('/connect/token', body, {headers: headers})
			.flatMap((response: Object) => {
				if (response) {
					this.setAuthUser(new AuthUser(response, this.authUser.isSso));
					return Observable.of(response);
				}

				this.logout();
				return Observable.of(null);
			})
			.catch(error => {
				return Observable.throw(error);
			})
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

	getAuthUser(): AuthUser {
		return this.authUser;
	}

	isLoggedIn(): boolean {
		return !!this.getAuthUser();
	}

	isRefreshTokenExpired(): boolean {
		if (!this.authUser) {
			return false
		}

		return new Date().getTime() > this.authUser.refreshTokenExpiration;
	}

	private objectToString(params: Object): string {
		return Object.keys(params).map(key => `${key}=${encodeURIComponent(params[key])}`).join('&');
	}

	private setAuthUser(authUser: AuthUser): void {
		this.authUser = authUser;
		localStorage.setItem(AUTH_USER_STORAGE_KEY, JSON.stringify(this.authUser));
		this.onChange.emit(this.authUser);
	}
}
