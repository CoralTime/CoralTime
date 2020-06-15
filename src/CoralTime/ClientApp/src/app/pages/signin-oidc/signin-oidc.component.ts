import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Component({
	selector: 'ct-signin-oidc',
	templateUrl: 'signin-oidc.component.html'
})

export class SignInOidcComponent implements OnInit {
	id_token: string;

	constructor(private auth: AuthGuard,
	            private authService: AuthService,
	            private loadingService: LoadingMaskService,
	            private route: ActivatedRoute,
	            private router: Router) {
	}

	ngOnInit() {
		this.route.fragment.subscribe((fragment) => {
			this.id_token = fragment.slice(fragment.indexOf('=') + 1, fragment.indexOf('&'));
			this.loginSSO(this.id_token);
		})
	}

	loginSSO(id_token: string): void {
		this.loadingService.addLoading();
		this.authService.loginSSO(this.id_token)
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.router.navigate(['/' + this.auth.url]);
				}
			);
	}
}
