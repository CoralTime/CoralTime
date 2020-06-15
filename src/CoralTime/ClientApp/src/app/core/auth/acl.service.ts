import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ImpersonationService } from '../../services/impersonation.service';

@Injectable()
export class AclService {
	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService) {
	}

	isGranted(policy: string): boolean {
		if (!this.authService.isLoggedIn()) {
			return false;
		}

		if (this.impersonationService.impersonationUser) {
			return this.isGrantedForRole(policy, this.impersonationService.impersonationUser.role);
		} else {
			return this.isGrantedForRole(policy, this.authService.authUser.role);
		}
	}

	isGrantedForRole(policy: string, role: string): boolean {
		var policies = this.authService.roles[role] as string;
		var isGranted = (policies && policies.indexOf(policy) != -1);
		return isGranted;
	}
}
