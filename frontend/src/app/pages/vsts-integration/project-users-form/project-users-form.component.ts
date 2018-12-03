import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { PagedResult } from '../../../services/odata';
import { UserProject } from '../../../models/user-project';
import { NotificationService } from '../../../core/notification.service';
import { ROWS_ON_PAGE } from '../../../core/constant.service';
import { VstsProjectConnection } from '../../../models/vsts-project-connection';
import { VstsIntegrationService } from '../../../services/vsts-integration.service';

@Component({
	selector: 'ct-project-users-form',
	templateUrl: 'project-users-form.component.html'
})

export class ProjectUsersFormComponent implements OnInit {
	@Input() connection: VstsProjectConnection;
	@ViewChild('grid') gridContainer: ElementRef;

	filterStr: string = '';
	resizeObservable: Subject<any> = new Subject();
	wrapperHeightObservable: Subject<any> = new Subject();

	assignedUsersPagedResult: PagedResult<UserProject>;
	updatingAssignedUsersGrid: boolean = false;
	isAllAssignedUsers: boolean = false;
	private assignedUsersSubject = new Subject<any>();
	private assignedUsersLastEvent: any;

	constructor(private notificationService: NotificationService,
	            private vstsIntegrationService: VstsIntegrationService) {
	}

	ngOnInit() {
		this.loadAssignedUsers();

		this.wrapperHeightObservable.debounceTime(100).subscribe(() => {
			this.resizeObservable.next();
		});
	}

	// ASSIGNED USERS GRID

	loadAssignedUsers(): void {
		this.assignedUsersSubject.debounceTime(500).switchMap(() => {
			return this.vstsIntegrationService.getConnectionsMembersWithCount(this.assignedUsersLastEvent, this.filterStr, this.connection.id);
		})
			.subscribe((res: PagedResult<UserProject>) => {
					if (!this.assignedUsersPagedResult || !this.assignedUsersLastEvent.first || this.updatingAssignedUsersGrid) {
						this.assignedUsersPagedResult = res;
					} else {
						this.assignedUsersPagedResult.data = this.assignedUsersPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.connection.membersCount = this.assignedUsersPagedResult.count;
					}

					this.assignedUsersLastEvent.first = this.assignedUsersPagedResult.data.length;
					this.updatingAssignedUsersGrid = false;
					this.wrapperHeightObservable.next();
					this.checkIsAllAssignedUsers();
				},
				() => this.notificationService.danger('Error loading users.')
			);
	}

	onAssignedUsersEndScroll(): void {
		if (!this.isAllAssignedUsers) {
			this.updateAssignedUsers();
		}
	}

	updateAssignedUsers(event = null, updatePage?: boolean): void {
		if (event) {
			this.assignedUsersLastEvent = event;
		}
		if (updatePage) {
			this.updatingAssignedUsersGrid = updatePage;
			this.assignedUsersLastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllAssignedUsers = false;
			this.assignedUsersPagedResult = null;
			this.resizeObservable.next(true);
		}
		this.assignedUsersLastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllAssignedUsers) {
			return;
		}

		this.assignedUsersSubject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllAssignedUsers(): void {
		if (this.assignedUsersPagedResult && this.assignedUsersPagedResult.data.length >= this.assignedUsersPagedResult.count) {
			this.isAllAssignedUsers = true;
		}
	}

	// GENERAL

	onResize(): void {
		this.resizeObservable.next();
	}
}
