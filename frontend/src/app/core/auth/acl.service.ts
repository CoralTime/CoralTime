import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Permissions } from './permissions';
import { ImpersonationService } from '../../services/impersonation.service';

@Injectable()
export class AclService {
	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService) {
	}

	isGranted(role: string): boolean {
		if (!this.authService.isLoggedIn()) {
			return false;
		}

		if (typeof Permissions[role] === 'undefined') {
			throw new Error('Role "' + role + '" doesn\'t exists.');
		}

		if (this.impersonationService.impersonationUser) {
			return !(Permissions[role] % this.impersonationService.impersonationUser.getPermissionRole());
		} else {
			return !(Permissions[role] % this.authService.authUser.role);
		}
	}
}
