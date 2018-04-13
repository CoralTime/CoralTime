import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';

import { AuthService } from './auth.service';
import { AclService } from './acl.service';

@Injectable()
export class AuthGuard implements CanActivate {
	url: string = 'calendar';

	constructor(private aclService: AclService,
	            private authService: AuthService,
	            private router: Router) {
	}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		let url: string = state.url;
		let role: string = route.data['role'];
		this.url = url;

		return this.checkLogin(url, role);
	}

	checkLogin(url: string, role: string): boolean {
		if (!this.authService.isLoggedIn()) {
			this.authService.logout();
			return false;
		}

		if (role && !this.aclService.isGranted(role)) {
			this.router.navigate(['/']);
			return false;
		}

		return true;
	}
}
