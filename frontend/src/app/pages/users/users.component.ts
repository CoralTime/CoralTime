import { UserProjectAssignmentComponent } from './project-assignment/project-assignment.component';
import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

import { UsersFormComponent } from './form/users-form.component';
import { User } from '../../models/user';
import { UsersService } from '../../services/users.service';
import { NotificationService } from '../../core/notification.service';
import { PagedResult } from '../../services/odata/query';
import { Subject } from 'rxjs';
import { ImpersonationService } from '../../services/impersonation.service';
import { AuthService } from '../../core/auth/auth.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';

@Component({
	selector: 'ct-users',
	templateUrl: 'users.component.html'
})

export class UsersComponent implements OnInit {
	currentUserId: number;
	impersonateUserId: number;
	isActiveTab: boolean = true;
	isAllUsers: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<User>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private subject = new Subject<any>();
	private lastEvent: any;

	private dialogRef: MatDialogRef<UsersFormComponent>;
	private dialogProjectAssignmentRef: MatDialogRef<UserProjectAssignmentComponent>;

	constructor(private authService: AuthService,
	            private dialog: MatDialog,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private userService: UsersService) {
		this.impersonationService.checkImpersonationRole('users');
	}

	ngOnInit() {
		this.currentUserId = this.authService.authUser.id;
		this.impersonateUserId = this.impersonationService.impersonationId;
		this.getUsers();
	}

	impersonateMember(user: User): void {
		this.impersonationService.impersonateMember(user);
	}

	onEndScroll(): void {
		this.checkIsAllUsers();

		if (!this.isAllUsers) {
			this.loadLazy();
		}
	}

	getUsers(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.userService.getUsersWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
		})
			.subscribe(
				(res: PagedResult<User>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}
					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
				},
				error => this.notificationService.danger('Error loading Users.')
			);
	}

	loadLazy(event = null, updatePage?: boolean): void {
		this.checkIsAllUsers();

		if (event) {
			this.lastEvent = event;
			this.isAllUsers = false;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.lastEvent.first = 0;
			this.isAllUsers = false;
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllUsers) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllUsers(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllUsers = true;
		}
	}

	openUserDialog(user: User = null): void {
		if (!this.authService.isLoggedIn()) {
			this.authService.logout();
			return;
		}

		this.dialogRef = this.dialog.open(UsersFormComponent);
		this.dialogRef.componentInstance.user = user;
		this.dialogRef.componentInstance.onSaved.subscribe((response) => {
			this.dialogRef.close();
			this.onSaved(response);
		});
	}

	openProjectAssignmentDialog(user: User = null): void {
		this.dialogProjectAssignmentRef = this.dialog.open(UserProjectAssignmentComponent);
		this.dialogProjectAssignmentRef.componentInstance.user = user;
	}

	onSaved(response: any): void {
		if (response.error) {
			this.notificationService.danger('Error saving user.');
			return;
		}

		if (response.isNewUser) {
			this.notificationService.success('New user has been successfully created.');
		} else {
			this.notificationService.success('User has been successfully changed.');
		}

		this.loadLazy(null, true);
	}

	toggleTab(isActiveTab: boolean): void {
		if (this.lastEvent) {
			this.lastEvent.first = 0;
			this.lastEvent.pageCount = 1;
		}

		this.isActiveTab = isActiveTab;
		this.loadLazy(null, true);
	}

	onResize(): void {
		this.resizeObservable.next();
	}
}
