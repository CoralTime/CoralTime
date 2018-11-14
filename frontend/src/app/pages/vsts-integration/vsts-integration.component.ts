import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Subject } from 'rxjs';
import { User } from '../../models/user';
import { PagedResult } from '../../services/odata';
import { AclService } from '../../core/auth/acl.service';
import { AuthService } from '../../core/auth/auth.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';
import { NotificationService } from '../../core/notification.service';

@Component({
	selector: 'ct-vsts-integration',
	templateUrl: 'vsts-integration.component.html'
})

export class VstsIntegrationComponent implements OnInit {
	isAllProjects: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<any>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private subject = new Subject<any>();
	private lastEvent: any;

	private dialogRef: MatDialogRef<any>;
	private dialogProjectAssignmentRef: MatDialogRef<any>;

	constructor(private aclService: AclService,
	            private authService: AuthService,
	            private dialog: MatDialog,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		this.getUsers();
	}

	// GRID DISPLAYING

	loadLazy(event = null, updatePage?: boolean): void {
		if (event) {
			this.lastEvent = event;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.lastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllProjects = false;
			this.pagedResult = null;
			this.resizeObservable.next(true);
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllProjects) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	onEndScroll(): void {
		if (!this.isAllProjects) {
			this.loadLazy();
		}
	}

	private checkIsAllUsers(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllProjects = true;
		}
	}

	private getUsers(): void {
		// this.subject.debounceTime(500).switchMap(() => {
		// 	return this.userService.getUsersWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
		// })
		// 	.subscribe((res: PagedResult<User>) => {
		// 			if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
		// 				this.pagedResult = res;
		// 			} else {
		// 				this.pagedResult.data = this.pagedResult.data.concat(res.data);
		// 			}
		//
		// 			this.lastEvent.first = this.pagedResult.data.length;
		// 			this.updatingGrid = false;
		// 			this.checkIsAllUsers();
		// 		},
		// 		() => this.notificationService.danger('Error loading Users.')
		// 	);
	}

	// FORM

	openUserDialog(user: User = null): void {
		if (!this.authService.isLoggedIn()) {
			this.authService.logout();
			return;
		}
		if (!this.aclService.isGranted('roleEditMember')) {
			return;
		}

		// this.dialogRef = this.dialog.open(UsersFormComponent);
		this.dialogRef.componentInstance.user = user;
		this.dialogRef.componentInstance.onSaved.subscribe((response) => {
			this.dialogRef.close();
			this.onSaved(response);
		});
	}

	openProjectAssignmentDialog(user: User = null): void {
		// this.dialogProjectAssignmentRef = this.dialog.open(UserProjectAssignmentComponent);
		this.dialogProjectAssignmentRef.componentInstance.user = user;
	}

	private onSaved(response: any): void {
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

	// GENERAL

	onResize(): void {
		this.resizeObservable.next();
	}
}
