import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { AuthUser } from '../../../core/auth/auth-user';
import { Project } from '../../../models/project';
import { ProjectRole } from '../../../models/project-role';
import { User } from '../../../models/user';
import { UserProject } from '../../../models/user-project';
import { AuthService } from '../../../core/auth/auth.service';
import { ROWS_ON_PAGE } from '../../../core/constant.service';
import { NotificationService } from '../../../core/notification.service';
import { ProjectRolesService } from '../../../services/project-roles.service';
import { UsersService } from '../../../services/users.service';
import { SettingsService } from '../../../services/settings.service';
import { PagedResult } from '../../../services/odata';
import { ArrayUtils } from '../../../core/object-utils';

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
	projectRoles: ProjectRole[];
	resizeObservable: Subject<any> = new Subject();
	wrapperHeightObservable: Subject<any> = new Subject();

	assignedUsersPagedResult: PagedResult<UserProject>;
	updatingAssignedUsersGrid: boolean;
	isAllAssignedUsers: boolean;
	isAssignedUsersLoading: boolean;
	private assignedUsersSubject = new Subject<any>();
	private assignedUsersLastEvent: any;

	notAssignedUsersPagedResult: PagedResult<User>;
	updatingNotAssignedUsersGrid: boolean;
	isAllNotAssignedUsers: boolean;
	isNotAssignedUsersLoading: boolean;
	private notAssignedUsersLastEvent: any;
	private notAssignedUsersSubject = new Subject<any>();

	constructor(private authService: AuthService,
	            private notificationService: NotificationService,
	            private projectRolesService: ProjectRolesService,
	            private settingsService: SettingsService,
	            private usersService: UsersService) {
	}

	ngOnInit() {
		this.authUser = this.authService.authUser;
		this.getProjectRoles();
		this.loadAssignedUsers();
		this.loadNotAssignedUsers();

		this.wrapperHeightObservable.debounceTime(50).subscribe(() => {
			if (!this.isAssignedUsersLoading && !this.isNotAssignedUsersLoading) {
				this.changeScrollableContainer();
				this.resizeObservable.next();
			}
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
			this.isAssignedUsersLoading = true;
			return this.usersService.getProjectUsersWithCount(this.assignedUsersLastEvent, this.filterStr, this.project.id);
		})
			.subscribe((res: PagedResult<UserProject>) => {
					if (!this.assignedUsersPagedResult || !this.assignedUsersLastEvent.first || this.updatingAssignedUsersGrid) {
						this.assignedUsersPagedResult = res;
					} else {
						this.assignedUsersPagedResult.data = this.assignedUsersPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.project.membersCount = this.assignedUsersPagedResult.count;
					}

					this.assignedUsersLastEvent.first = this.assignedUsersPagedResult.data.length;
					this.isAssignedUsersLoading = false;
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
			this.isNotAssignedUsersLoading = true;
			return this.usersService.getUnassignedUsersWithCount(this.notAssignedUsersLastEvent, this.filterStr, this.project.id);
		})
			.subscribe((res: PagedResult<User>) => {
					if (!this.notAssignedUsersPagedResult || !this.notAssignedUsersLastEvent.first || this.updatingNotAssignedUsersGrid) {
						this.notAssignedUsersPagedResult = res;
					} else {
						this.notAssignedUsersPagedResult.data = this.notAssignedUsersPagedResult.data.concat(res.data);
					}

					this.notAssignedUsersLastEvent.first = this.notAssignedUsersPagedResult.data.length;
					this.isNotAssignedUsersLoading = false;
					this.updatingNotAssignedUsersGrid = false;
					this.wrapperHeightObservable.next();
					this.checkIsAllUnassignedUsers();
				},
				() => this.notificationService.danger('Error loading users.')
			);
	}

	onNotAssignedUsersEndScroll(): void {
		if (!this.isAllNotAssignedUsers) {
			this.updateNotAssignedUsers();
		}
	}

	updateNotAssignedUsers(event = null, updatePage?: boolean): void {
		if (event) {
			this.notAssignedUsersLastEvent = event;
		}
		if (updatePage) {
			this.updatingNotAssignedUsersGrid = updatePage;
			this.notAssignedUsersLastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllNotAssignedUsers = false;
			this.notAssignedUsersPagedResult = null;
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

	addToProject(user: User, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.assignUserToProject(this.project.id, user.id, this.defaultProjectRole.id)
			.subscribe(() => {
					this.notificationService.success('User successfully assigned.');
					this.updateAssignedUsers(null, true);
					this.updateNotAssignedUsers(null, true);
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger('Error adding user.');
				}
			);
	}

	assignToPublic(userProject: UserProject, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.assignProjectToUser(userProject.memberId, userProject.projectId, userProject.roleId)
			.finally(() => target.classList.remove('ct-loading'))
			.subscribe(() => {
					this.notificationService.success('Access level has been changed.');
					this.updateAssignedUsers(null, true);
					this.updateNotAssignedUsers(null, true);
				},
				() => {
					this.notificationService.danger('Access level has not been changed.');
				}
			);
	}

	changeRole(userProject: UserProject, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.changeRole(userProject.id, userProject.role.id)
			.finally(() => target.classList.remove('ct-loading'))
			.subscribe(() => {
					this.notificationService.success('Access level has been changed.');
					this.updateAssignedUsers(null, true);
					this.updateNotAssignedUsers(null, true);
				},
				() => {
					this.notificationService.danger('Access level has not been changed.');
				}
			);
	}

	removeFromProject(userProject: UserProject, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.removeFromProject(userProject)
			.subscribe(() => {
					this.notificationService.success('User was removed from project.');
					this.updateAssignedUsers(null, true);
					this.updateNotAssignedUsers(null, true);
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger('Error removing user.');
				}
			);
	}

	onResize(): void {
		this.wrapperHeightObservable.next();
	}

	private changeScrollableContainer(): void {
		const gridContainer = this.gridContainer.nativeElement;
		const grids = gridContainer.querySelectorAll('.ct-grid-container');
		const wrappers = gridContainer.querySelectorAll('.ui-datatable-scrollable-body');

		wrappers.forEach((wrapper, i) => {
			grids[i].removeAttribute('style');
			if (wrapper.scrollHeight > wrapper.children[0].scrollHeight) {
				grids[i].setAttribute('style', 'flex: initial');
			}
		});
	}
}
