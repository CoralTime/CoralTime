import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable()
export class NotAuthGuard implements CanActivate {
	constructor(private authService: AuthService,
	            private router: Router) { }

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		let url: string = state.url;

		return this.checkLogin(url);
	}

	checkLogin(url: string): boolean {
		if (!this.authService.isLoggedIn()) {
			this.authService.logout(true);
			return true;
		}

		this.router.navigate(['/calendar']);
		this.authService.onChange.emit(this.authService.authUser);

		return false;
	}
}
