import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { PagedResult } from '../../../services/odata';
import { UserProject } from '../../../models/user-project';
import { NotificationService } from '../../../core/notification.service';
import { ProjectRolesService } from '../../../services/project-roles.service';
import { SettingsService } from '../../../services/settings.service';
import { UsersService } from '../../../services/users.service';
import { ArrayUtils } from '../../../core/object-utils';
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
	            private projectRolesService: ProjectRolesService,
	            private settingsService: SettingsService,
	            private vstsIntegrationService: VstsIntegrationService) {
	}

	ngOnInit() {
		this.loadAssignedUsers();

		this.wrapperHeightObservable.debounceTime(100).subscribe(() => {
			this.changeScrollableContainer();
			this.resizeObservable.next();
		});
	}

	// ASSIGNED USERS GRID

	loadAssignedUsers(): void {
		this.assignedUsersSubject.debounceTime(500).switchMap(() => {
			return this.vstsIntegrationService.getConnectionsMembersWithCount(this.assignedUsersLastEvent, this.filterStr, this.connection.id);
		})
			.subscribe((res: PagedResult<UserProject>) => {
					console.log(res);
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

	private changeScrollableContainer(): void {
		const HEIGHT = 325;
		let grid = this.gridContainer.nativeElement;
		let wrappers = grid.querySelectorAll('.ui-datatable-scrollable-body');

		if (wrappers.length === 1) {
			return;
		}

		wrappers[0].setAttribute('style', 'max-height: calc((100vh - ' + HEIGHT + 'px)/2)');
		wrappers[1].setAttribute('style', 'max-height: calc((100vh - ' + HEIGHT + 'px)/2)');

		if (wrappers[0].scrollHeight < (window.innerHeight - HEIGHT) / 2) {
			wrappers[1].setAttribute('style', 'max-height: calc(100vh - ' + HEIGHT + 'px - ' + wrappers[0].scrollHeight + 'px)');
		}
		if (wrappers[1].scrollHeight < (window.innerHeight - HEIGHT) / 2) {
			wrappers[0].setAttribute('style', 'max-height: calc(100vh - ' + HEIGHT + 'px - ' + wrappers[1].scrollHeight + 'px)');
		}
	}
}
