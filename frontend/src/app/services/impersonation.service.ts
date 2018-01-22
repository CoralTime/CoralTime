import { EventEmitter, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';

const IMPERSONATION_USER_STORAGE_KEY = 'IMPERSONATION_USER';

@Injectable()
export class ImpersonationService {
	impersonationMember: string = null;
	impersonationId: number;
	impersonationUser: User;

	onChange: EventEmitter<any> = new EventEmitter();

	constructor(private router: Router) {
		this.getStorage();
	}

	impersonateMember(user: User): void {
		this.impersonationMember = user.userName;
		this.impersonationId = user.id;
		this.impersonationUser = user;
		this.onChange.emit(user.isAdmin ? 2 : user.isManager ? 1 : 0);
		this.setStorage(user);
		this.router.navigate(['/calendar']);
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
		if (localStorage.hasOwnProperty(IMPERSONATION_USER_STORAGE_KEY)) {
			this.impersonateMember(JSON.parse(localStorage.getItem(IMPERSONATION_USER_STORAGE_KEY)));
		}
	}

	setStorage(user: User): void {
		localStorage.setItem(IMPERSONATION_USER_STORAGE_KEY, JSON.stringify(user));
	}

	clearStorage(): void {
		localStorage.removeItem(IMPERSONATION_USER_STORAGE_KEY);
	}

	isNotAdmin(): boolean {
		return !!this.impersonationUser && !this.impersonationUser.isAdmin;
	}

	isNotManager(): boolean {
		return !!this.impersonationUser && !this.impersonationUser.isManager;
	}

	checkImpersonationRole(page: string): void {
		let adminPages = ['clients', 'users', 'tasks'];
		let managerPages = ['projects'];
		if ((managerPages.indexOf(page) && this.isNotManager()) && (adminPages.indexOf(page) && this.isNotAdmin())) {
			this.router.navigate(['/']);
		}
	}
}
