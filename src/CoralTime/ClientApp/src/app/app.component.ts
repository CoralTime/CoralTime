import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from './core/auth/auth.service';
import { ImpersonationService } from './services/impersonation.service';

@Component({
	selector: 'ct-root',
	templateUrl: 'app.component.html'
})

export class AppComponent implements OnInit {
	constructor(private authService: AuthService,
	            public impersonationService: ImpersonationService,
	            translate: TranslateService) {
		// this language will be used as a fallback when a translation isn't found in the current language
		translate.setDefaultLang('en');

		// the lang to use, if the lang isn't available, it will use the current loader to get them
		translate.use('en');
	}

	ngOnInit() {
		if (!this.isLoggedIn()) {
			this.stopImpersonation(true);
		}
	}

	isLoggedIn(): boolean {
		return this.authService.isLoggedIn();
	}

	stopImpersonation(isLogOut?: boolean): void {
		this.impersonationService.stopImpersonation(isLogOut);
	}
}
