import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Project } from '../../../models/project';
import { UserProject } from '../../../models/user-project';
import { UsersService } from '../../../services/users.service';
import { NotificationService } from '../../../core/notification.service';
import { AuthUser } from '../../../core/auth/auth-user';
import { AuthService } from '../../../core/auth/auth.service';
import { ProjectRolesService } from '../../../services/project-roles.service';
import { SettingsService } from '../../../services/settings.service';
import { ProjectRole } from '../../../models/project-role';
import { User } from '../../../models/user';
import { PagedResult } from '../../../services/odata/query';
import { Subject } from 'rxjs/Subject';
import { ArrayUtils } from '../../../core/object-utils';
import { ROWS_ON_PAGE } from '../../../core/constant.service';

@Component({
	selector: 'ct-project-members',
	templateUrl: 'project-members.component.html'
})

export class ProjectUsersComponent implements OnInit {
	@Input() project: Project;
	@ViewChild('grid') gridContainer: ElementRef;

	authUser: AuthUser;
	defaultProjectRole: ProjectRole;
	filterStr: string = '';
	isRequestLoading: boolean = false;
	projectRoles: ProjectRole[];
	resizeObservable: Subject<any> = new Subject();
	wrapperHeightObservable: Subject<any> = new Subject();

	assignedUsersPagedResult: PagedResult<UserProject>;
	updatingAssignedUsersGrid: boolean = false;
	isAllAssignedUsers: boolean = false;
	private assignedUsersSubject = new Subject<any>();
	private assignedUsersLastEvent: any;

	notAssignedUsersPagedResult: PagedResult<User>;
	updatingNotAssignedUsersGrid: boolean = false;
	isAllNotAssignedUsers: boolean = false;
	private notAssignedUsersLastEvent: any;
	private notAssignedUsersSubject = new Subject<any>();

	constructor(private usersService: UsersService,
	            private notificationService: NotificationService,
	            private authService: AuthService,
	            private projectRolesService: ProjectRolesService,
	            private settingsService: SettingsService) {
	}

	ngOnInit() {
		this.authUser = this.authService.authUser;
		this.getProjectRoles();
		this.loadAssignedUsers();
		this.loadNotAssignedUsers();

		this.wrapperHeightObservable.debounceTime(100).subscribe(() => {
			this.changeScrollableContainer();
			this.resizeObservable.next();
		});
	}

	getProjectRoles(): void {
		this.projectRolesService.getProjectRoles().subscribe((projectRoles) => {
			this.projectRoles = projectRoles;

			this.settingsService.getDefaultProjectRoleName().subscribe(projectRoleName => {
				this.defaultProjectRole = ArrayUtils.findByProperty(this.projectRoles, 'name', projectRoleName);
			});
		});
	}

	// ASSIGNED USERS GRID

	loadAssignedUsers(): void {
		this.assignedUsersSubject.debounceTime(500).switchMap(() => {
			return this.usersService.getProjectUsersWithCount(this.assignedUsersLastEvent, this.filterStr, this.project.id);
		})
			.subscribe(
				(res: PagedResult<UserProject>) => {
					if (!this.assignedUsersPagedResult || !this.assignedUsersLastEvent.first || this.updatingAssignedUsersGrid) {
						this.assignedUsersPagedResult = res;
					} else {
						this.assignedUsersPagedResult.data = this.assignedUsersPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.project.membersCount = this.assignedUsersPagedResult.count;
					}

					this.assignedUsersLastEvent.first = this.assignedUsersPagedResult.data.length;
					this.updatingAssignedUsersGrid = false;
					this.wrapperHeightObservable.next();
				},
				error => this.notificationService.danger('Error loading users.')
			);
	}

	onAssignedUsersEndScroll(): void {
		this.checkIsAllAssignedUsers();

		if (!this.isAllAssignedUsers) {
			this.updateAssignedUsers();
		}
	}

	updateAssignedUsers(event = null, updatePage?: boolean): void {
		this.checkIsAllAssignedUsers();

		if (event) {
			this.assignedUsersLastEvent = event;
			this.isAllAssignedUsers = false;
		}
		if (updatePage) {
			this.updatingAssignedUsersGrid = updatePage;
			this.assignedUsersLastEvent.first = 0;
			this.isAllAssignedUsers = false;
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

	// NOT ASSIGNED USERS GRID

	loadNotAssignedUsers(): void {
		this.notAssignedUsersSubject.debounceTime(500).switchMap(() => {
			return this.usersService.getUnassignedUsersWithCount(this.notAssignedUsersLastEvent, this.filterStr, this.project.id);
		})
			.subscribe(
				(res: PagedResult<User>) => {
					if (!this.notAssignedUsersPagedResult || !this.notAssignedUsersLastEvent.first || this.updatingNotAssignedUsersGrid) {
						this.notAssignedUsersPagedResult = res;
					} else {
						this.notAssignedUsersPagedResult.data = this.notAssignedUsersPagedResult.data.concat(res.data);
					}
					this.notAssignedUsersLastEvent.first = this.notAssignedUsersPagedResult.data.length;
					this.updatingNotAssignedUsersGrid = false;
					this.wrapperHeightObservable.next();
				},
				error => this.notificationService.danger('Error loading users.')
			);
	}

	onNotAssignedUsersEndScroll(): void {
		this.checkIsAllUnassignedUsers();

		if (!this.isAllNotAssignedUsers) {
			this.updateNotAssignedUsers();
		}
	}

	updateNotAssignedUsers(event = null, updatePage?: boolean): void {
		this.checkIsAllUnassignedUsers();

		if (event) {
			this.notAssignedUsersLastEvent = event;
			this.isAllNotAssignedUsers = false;
		}
		if (updatePage) {
			this.updatingNotAssignedUsersGrid = updatePage;
			this.notAssignedUsersLastEvent.first = 0;
			this.isAllNotAssignedUsers = false;
		}
		this.notAssignedUsersLastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllNotAssignedUsers) {
			return;
		}

		this.notAssignedUsersSubject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllUnassignedUsers(): void {
		if (this.notAssignedUsersPagedResult && this.notAssignedUsersPagedResult.data.length >= this.notAssignedUsersPagedResult.count) {
			this.isAllNotAssignedUsers = true;
		}
	}

	// GENERAL

	addToProject(user: User): void {
		this.isRequestLoading = true;
		this.usersService.assignUserToProject(this.project.id, user.id, this.defaultProjectRole.id)
			.subscribe(
				() => {
					this.isRequestLoading = false;
					this.notificationService.success('User successfully assigned.');
					this.filterStr = '';
					this.updateAssignedUsers(null, true);
					this.updateNotAssignedUsers(null, true);
				},
				() => {
					this.notificationService.danger('Error adding user.');
				}
			);
	}

	assignToPublic(userProject: UserProject): void {
		this.usersService.assignProjectToUser(userProject.memberId, userProject.projectId, userProject.roleId).subscribe(
			() => {
				this.notificationService.success('Access level has been changed.');
				this.updateAssignedUsers(null, true);
				this.updateNotAssignedUsers(null, true);
			},
			() => {
				this.notificationService.danger('Access level has not been changed.');
			}
		);
	}

	changeRole(userProject: UserProject): void {
		this.usersService.changeRole(userProject.id, userProject.role.id).subscribe(
			() => {
				this.notificationService.success('Access level has been changed.');
				this.updateAssignedUsers(null, true);
				this.updateNotAssignedUsers(null, true);
			},
			() => {
				this.notificationService.danger('Access level has not been changed.');
			}
		);
	}

	removeFromProject(userProject: UserProject): void {
		this.isRequestLoading = true;
		this.usersService.removeFromProject(userProject).subscribe(
			() => {
				this.isRequestLoading = false;
				this.notificationService.success('User was removed from project.');
				this.filterStr = '';
				this.updateAssignedUsers(null, true);
				this.updateNotAssignedUsers(null, true);
			},
			() => {
				this.notificationService.danger('Error removing user.');
			}
		);
	}

	onResize(): void {
		this.resizeObservable.next();
	}

	private changeScrollableContainer(): void {
		let grid = this.gridContainer.nativeElement;
		let wrappers = grid.querySelectorAll('.ui-datatable-scrollable-body');

		if (wrappers.length === 1) {
			return;
		}

		wrappers[0].setAttribute('style', 'max-height: calc((90vh - 180px)/2)');
		wrappers[1].setAttribute('style', 'max-height: calc((90vh - 180px)/2)');

		if (wrappers[0].scrollHeight < (window.innerHeight * 0.9 - 180) / 2) {
			wrappers[1].setAttribute('style', 'max-height: calc(90vh - 180px - ' + wrappers[0].scrollHeight + 'px)');
		}
		if (wrappers[1].scrollHeight < (window.innerHeight * 0.9 - 180) / 2) {
			wrappers[0].setAttribute('style', 'max-height: calc(90vh - 180px - ' + wrappers[1].scrollHeight + 'px)');
		}
	}
}
