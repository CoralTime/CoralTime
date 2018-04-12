import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { AuthGuard } from '../../core/auth/auth-guard.service';

@Component({
	templateUrl: 'login-demo.component.html'
})

export class LoginDemoComponent {
	constructor(private authService: AuthService,
	            private auth: AuthGuard,
	            private router: Router) {
	}

	loginByData(username: string, password: string): void {
		this.authService.login(username, password)
			.subscribe(
				data => this.router.navigate(['/' + this.auth.url])
			);
	}
}
