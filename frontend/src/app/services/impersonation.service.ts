import { EventEmitter, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';

@Injectable()
export class ImpersonationService {
	impersonationId: number;
	impersonationMember: string = null;
	impersonationUser: User;

	onChange: EventEmitter<any> = new EventEmitter();

	constructor(private router: Router) {
		this.getStorage();
	}

	impersonateMember(user: User, navigate?: boolean): void {
		this.impersonationMember = user.userName;
		this.impersonationId = user.id;
		this.impersonationUser = user;
		this.onChange.emit(user.isAdmin ? 2 : user.isManager ? 1 : 0);
		this.setStorage(user);
		if (navigate) {
			this.router.navigate(['/calendar']);
		}
	}

	stopImpersonation(isLogOut?: boolean): void {
		this.impersonationMember = null;
		this.impersonationUser = null;
		this.impersonationId = null;
		this.onChange.emit(null);
		this.clearStorage();
		if (!isLogOut) {
			this.router.navigate(['/calendar']);
		}
	}

	getStorage(): void {
		if (sessionStorage.hasOwnProperty('IMPERSONATION_USER')) {
			this.impersonateMember(new User(JSON.parse(sessionStorage.getItem('IMPERSONATION_USER'))));
		}
	}

	setStorage(user: User): void {
		sessionStorage.setItem('IMPERSONATION_USER', JSON.stringify(user));
	}

	clearStorage(): void {
		sessionStorage.removeItem('IMPERSONATION_USER');
	}
}
