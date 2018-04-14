import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { AuthGuard } from '../../core/auth/auth-guard.service';

import { AdalConfig, Authentication } from 'adal-ts';
import { AzureSettings, LoginSettings } from './login.service';

@Component({
	templateUrl: 'login.component.html'
})

export class LoginComponent implements OnInit {
	enableAzure: boolean = false;
	errorMessage: string;
	password: string;
	username: string;

	private config: AdalConfig;

	constructor(private authService: AuthService,
	            private auth: AuthGuard,
	            private route: ActivatedRoute,
	            private router: Router) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { loginSettings: LoginSettings }) => {
			if (data.loginSettings.enableAzure) {
				this.enableAzure = true;
				this.createConfig(data.loginSettings.azureSettings);
			}
		});
	}

	login(): void {
		this.errorMessage = null;
		this.authService.login(this.username, this.password)
			.subscribe(
				data => this.router.navigateByUrl('/' + this.auth.url),
				error => this.handleError(error)
			);
	}

	loginSSO(): void {
		let context = Authentication.getContext(this.config);
		context.login();
	}

	private handleError(error: any): void {
		if (this.username.length < 1 && this.password.length < 1) {
			this.errorMessage = 'Login and password are required!';
		} else if (this.username.length < 1 && this.password.length > 1) {
			this.errorMessage = 'Login is required!';
		} else if (this.password.length < 1 && this.username.length > 1) {
			this.errorMessage = 'Password is required!';
		} else {
			this.errorMessage = error.status === 400 ? 'Invalid username or password' : 'Server error';
		}
	}

	private createConfig(azureSettings: AzureSettings): void {
		this.config = {
			tenant: azureSettings.tenant,
			clientId: azureSettings.clientId,
			postLogoutRedirectUrl: window.location.origin + '/',
			redirectUri: azureSettings.redirectUrl
		};

		return;
	}
}
