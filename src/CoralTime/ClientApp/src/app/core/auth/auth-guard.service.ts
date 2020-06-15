import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AclService } from './acl.service';
import { AuthService } from './auth.service';

@Injectable()
export class AuthGuard implements CanActivate {
	url: string = 'calendar';

	constructor(private aclService: AclService,
	            private authService: AuthService,
	            private router: Router) {
	}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		this.url = state.url;
		return this.checkLogin(state.url, route.data['role']);
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
