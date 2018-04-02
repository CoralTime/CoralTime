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
			// this.authUser.accessToken = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjA2RDNFNDZFOTEwNzNDNUQ0QkMyQzk5ODNCRTlGRjQ0OENGNjQwRDQiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJCdFBrYnBFSFBGMUx3c21ZTy1uX1JJejJRTlEifQ.eyJuYmYiOjE0OTg0MjQ4MDIsImV4cCI6MTQ5ODUxMTIwMiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJXZWJBUEkiXSwiY2xpZW50X2lkIjoiY29yYWx0aW1lYXBwIiwic3ViIjoiM2Q2M2NkOTItMjA0Ny00MzdhLTlhNzAtM2U1Y2FhNTQyN2JmIiwiYXV0aF90aW1lIjoxNDk4NDI0ODAxLCJpZHAiOiJsb2NhbCIsIm5hbWUiOiJBZG1pbiIsInJvbGUiOiJhZG1pbiIsIm5pY2tuYW1lIjoiVGVzdCBBZG1pbjEiLCJlbWFpbCI6ImNvcmFsdGltZWFkbWluQGNvcmFsdGVxLmNvbSIsInNjb3BlIjpbIm9wZW5pZCIsInByb2ZpbGUiLCJyb2xlcyIsIldlYkFQSSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJjdXN0b20iXX0.k5d0SWmdFOMUxkVVihxNqaArT8N2CjoRXqDp_0_Xy3PQWcRKV_AxQjGfH8teT3fgAyLro5zUBWH7RPMd1QzdZbOMR0u7flMfk2BHS9m0Yeua8O9NtET3ssVRcw45CfVOKEDZQunQQKVNyi5LjIEmMk6eWkgSwkrIykyYLQ0Ph0_V7xYmbcsaTvyZ1iAv7d5GO5VXUtn120tY4GxlYDNBYHzBBZm0wDEmeMsbiU2d4Q-Mukje8BH7gCJAftrRQAqKaMzFhhjjvMozC94IpM2rQAX9XHM7dET0VKWj3IIM3f_VOWmKOAjf6dC6Q4_PivKVnjOF93jgqxwMKT-Wl5ps4Q'
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
			console.log(222);
			return Observable.throw(new Error('User data not found.'));
		}

		let headers = new Headers();
		headers.append('Content-Type', 'application/x-www-form-urlencoded');

		let params = new URLSearchParams();
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
