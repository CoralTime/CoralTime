import { UserProject } from '../../../models/user-project';
import { NotificationService } from '../../../core/notification.service';
import { UsersService } from '../../../services/users.service';
import { User } from '../../../models/user';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { PagedResult } from '../../../services/odata/query';
import { Subject } from 'rxjs/Subject';
import { ProjectRole } from '../../../models/project-role';
import { Project } from '../../../models/project';
import { ArrayUtils } from '../../../core/object-utils';
import { SettingsService } from '../../../services/settings.service';
import { ProjectRolesService } from '../../../services/project-roles.service';
import { ROWS_ON_PAGE } from '../../../core/constant.service';

@Component({
	selector: 'ct-user-project-assignment',
	templateUrl: 'project-assignment.component.html'
})

export class UserProjectAssignmentComponent implements OnInit {
	@Input() user: User;
	@ViewChild('grid') gridContainer: ElementRef;

	defaultProjectRole: ProjectRole;
	filterStr: string = '';
	projectRoles: ProjectRole[];
	resizeObservable: Subject<any> = new Subject();
	isRequestLoading: boolean;
	wrapperHeightObservable: Subject<any> = new Subject();

	assignedProjectsPagedResult: PagedResult<UserProject>;
	updatingAssignedProjectsGrid: boolean = false;
	isAllAssignedProjects: boolean = false;
	private assignedProjectsSubject = new Subject<any>();
	private assignedProjectsLastEvent: any;

	notAssignedProjectsPagedResult: PagedResult<Project>;
	updatingNotAssignedProjectsGrid: boolean = false;
	isAllNotAssignedProjects: boolean = false;
	private notAssignedProjectsLastEvent: any;
	private notAssignedProjectsSubject = new Subject<any>();

	constructor(private notificationService: NotificationService,
	            private projectRolesService: ProjectRolesService,
	            private settingsService: SettingsService,
	            private usersService: UsersService) {
	}

	ngOnInit() {
		this.getProjectRoles();
		this.loadAssignedProjects();
		this.loadNotAssignedProjects();

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

	// ASSIGNED PROJECTS GRID

	loadAssignedProjects(): void {
		this.assignedProjectsSubject.debounceTime(500).switchMap(() => {
			return this.usersService.getUserProjectsWithCount(this.assignedProjectsLastEvent, this.filterStr, this.user.id);
		})
			.subscribe(
				(res: PagedResult<UserProject>) => {
					if (!this.assignedProjectsPagedResult || !this.assignedProjectsLastEvent.first || this.updatingAssignedProjectsGrid) {
						this.assignedProjectsPagedResult = res;
					} else {
						this.assignedProjectsPagedResult.data = this.assignedProjectsPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.user.projectsCount = this.assignedProjectsPagedResult.count;
					}

					this.assignedProjectsLastEvent.first = this.assignedProjectsPagedResult.data.length;
					this.updatingAssignedProjectsGrid = false;
					this.wrapperHeightObservable.next();
				},
				error => this.notificationService.danger('Error loading projects.')
			);
	}

	onAssignedProjectsEndScroll(): void {
		this.checkIsAllAssignedProjects();

		if (!this.isAllAssignedProjects) {
			this.updateAssignedProjects();
		}
	}

	updateAssignedProjects(event = null, updatePage?: boolean): void {
		this.checkIsAllAssignedProjects();

		if (event) {
			this.assignedProjectsLastEvent = event;
			this.isAllAssignedProjects = false;
		}
		if (updatePage) {
			this.updatingAssignedProjectsGrid = updatePage;
			this.assignedProjectsLastEvent.first = 0;
			this.isAllAssignedProjects = false;
		}
		this.assignedProjectsLastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllAssignedProjects) {
			return;
		}

		this.assignedProjectsSubject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllAssignedProjects(): void {
		if (this.assignedProjectsPagedResult && this.assignedProjectsPagedResult.data.length >= this.assignedProjectsPagedResult.count) {
			this.isAllAssignedProjects = true;
		}
	}

	// NOT ASSIGNED PROJECTS GRID

	loadNotAssignedProjects(): void {
		this.notAssignedProjectsSubject.debounceTime(500).switchMap(() => {
			return this.usersService.getUnassignedProjectsWithCount(this.notAssignedProjectsLastEvent, this.filterStr, this.user.id);
		})
			.subscribe(
				(res: PagedResult<Project>) => {
					if (!this.notAssignedProjectsPagedResult || !this.notAssignedProjectsLastEvent.first || this.updatingNotAssignedProjectsGrid) {
						this.notAssignedProjectsPagedResult = res;
					} else {
						this.notAssignedProjectsPagedResult.data = this.notAssignedProjectsPagedResult.data.concat(res.data);
					}
					this.notAssignedProjectsLastEvent.first = this.notAssignedProjectsPagedResult.data.length;
					this.updatingNotAssignedProjectsGrid = false;
					this.wrapperHeightObservable.next();
				},
				error => this.notificationService.danger('Error loading projects.')
			);
	}

	onNotAssignedProjectsEndScroll(): void {
		this.checkIsAllUnassignedProjects();

		if (!this.isAllNotAssignedProjects) {
			this.updateNotAssignedProjects();
		}
	}

	updateNotAssignedProjects(event = null, updatePage?: boolean): void {
		this.checkIsAllUnassignedProjects();

		if (event) {
			this.notAssignedProjectsLastEvent = event;
			this.isAllNotAssignedProjects = false;
		}
		if (updatePage) {
			this.updatingNotAssignedProjectsGrid = updatePage;
			this.notAssignedProjectsLastEvent.first = 0;
			this.isAllNotAssignedProjects = false;
		}
		this.notAssignedProjectsLastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllNotAssignedProjects) {
			return;
		}

		this.notAssignedProjectsSubject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllUnassignedProjects(): void {
		if (this.notAssignedProjectsPagedResult && this.notAssignedProjectsPagedResult.data.length >= this.notAssignedProjectsPagedResult.count) {
			this.isAllNotAssignedProjects = true;
		}
	}

	// GENERAL

	addToUser(project: Project): void {
		this.isRequestLoading = true;
		this.usersService.assignProjectToUser(this.user.id, project.id, this.defaultProjectRole.id)
			.subscribe(
				() => {
					this.isRequestLoading = false;
					this.notificationService.success('Project successfully assigned.');
					this.filterStr = '';
					this.updateAssignedProjects(null, true);
					this.updateNotAssignedProjects(null, true);
				},
				() => {
					this.notificationService.danger('Error adding project.');
				}
			);
	}

	assignToPublic(userProject: UserProject): void {
		this.usersService.assignProjectToUser(userProject.memberId, userProject.projectId, userProject.roleId)
			.subscribe(
				() => {
					this.notificationService.success('Access level has been changed.');
					this.updateAssignedProjects();
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
				this.updateAssignedProjects();
			},
			() => {
				this.notificationService.danger('Access level has not been changed.');
			}
		);
	}

	removeFromUser(userProject: UserProject): void {
		this.isRequestLoading = true;
		this.usersService.removeFromProject(userProject).subscribe(
			() => {
				this.isRequestLoading = false;
				this.notificationService.success('User was removed from project.');
				this.filterStr = '';
				this.updateAssignedProjects(null, true);
				this.updateNotAssignedProjects(null, true);
			},
			() => {
				this.notificationService.danger('Error removing project.');
			}
		);
	}

	onResize(): void {
		this.resizeObservable.next();
	}

	private changeScrollableContainer(): void {
		let grid = this.gridContainer.nativeElement;
		let wrappers = grid.querySelectorAll('.ui-datatable-scrollable-body');
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
