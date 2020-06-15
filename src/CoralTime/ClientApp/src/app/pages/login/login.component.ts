import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdalConfig, Authentication } from 'adal-ts';
import { AuthService } from '../../core/auth/auth.service';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { AzureSettings, LoginSettings } from './login.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';
import { AppInsightsService } from '@markpieszak/ng-application-insights';

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
	            private loadingService: LoadingMaskService,
	            private route: ActivatedRoute,
	            private router: Router,
                private appInsightsService: AppInsightsService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { loginSettings: LoginSettings }) => {
            this.setupAppInsights(data.loginSettings.instrumentationKey);
			if (data.loginSettings.enableAzure) {
				this.enableAzure = true;                
				this.createConfig(data.loginSettings.azureSettings);
			}
		});
	}

	login(): void {
		this.errorMessage = null;
		this.loadingService.addLoading();
		this.authService.login(this.username, this.password)
			.finally(() => this.loadingService.removeLoading())
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

        this.appInsightsService.trackException(
        	error, 
			'login.component', 
			{
				'login': this.username,
				'errorMessage': this.errorMessage,
				'error_description': error.error.error_description
            })
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
	
	private setupAppInsights(instrumentationKey: string ): void {
        localStorage.setItem('instrumentationKey', instrumentationKey);
		if (instrumentationKey!= null && instrumentationKey !='') {
            this.appInsightsService.config = {
                instrumentationKey: instrumentationKey
            };
            this.appInsightsService.init();
        }
	}
}
