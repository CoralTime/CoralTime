import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { ArrayUtils } from '../../../core/object-utils';
import { Project } from '../../../models/project';
import { ProjectRole } from '../../../models/project-role';
import { UserProject } from '../../../models/user-project';
import { User } from '../../../models/user';
import { ROWS_ON_PAGE } from '../../../core/constant.service';
import { NotificationService } from '../../../core/notification.service';
import { PagedResult } from '../../../services/odata';
import { ProjectRolesService } from '../../../services/project-roles.service';
import { UsersService } from '../../../services/users.service';
import { SettingsService } from '../../../services/settings.service';

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
			.subscribe((res: PagedResult<UserProject>) => {
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
					this.checkIsAllAssignedProjects();
				},
				() => this.notificationService.danger('Error loading projects.')
			);
	}

	onAssignedProjectsEndScroll(): void {
		if (!this.isAllAssignedProjects) {
			this.updateAssignedProjects();
		}
	}

	updateAssignedProjects(event = null, updatePage?: boolean): void {
		if (event) {
			this.assignedProjectsLastEvent = event;
		}
		if (updatePage) {
			this.updatingAssignedProjectsGrid = updatePage;
			this.assignedProjectsLastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllAssignedProjects = false;
			this.assignedProjectsPagedResult = null;
			this.resizeObservable.next(true);
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
			.subscribe((res: PagedResult<Project>) => {
					if (!this.notAssignedProjectsPagedResult || !this.notAssignedProjectsLastEvent.first || this.updatingNotAssignedProjectsGrid) {
						this.notAssignedProjectsPagedResult = res;
					} else {
						this.notAssignedProjectsPagedResult.data = this.notAssignedProjectsPagedResult.data.concat(res.data);
					}

					this.notAssignedProjectsLastEvent.first = this.notAssignedProjectsPagedResult.data.length;
					this.updatingNotAssignedProjectsGrid = false;
					this.wrapperHeightObservable.next();
					this.checkIsAllUnassignedProjects();
				},
				() => this.notificationService.danger('Error loading projects.')
			);
	}

	onNotAssignedProjectsEndScroll(): void {
		if (!this.isAllNotAssignedProjects) {
			this.updateNotAssignedProjects();
		}
	}

	updateNotAssignedProjects(event = null, updatePage?: boolean): void {
		if (event) {
			this.notAssignedProjectsLastEvent = event;
		}
		if (updatePage) {
			this.updatingNotAssignedProjectsGrid = updatePage;
			this.notAssignedProjectsLastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllNotAssignedProjects = false;
			this.notAssignedProjectsPagedResult = null;
			this.resizeObservable.next(true);
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

	addToUser(project: Project, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.assignProjectToUser(this.user.id, project.id, this.defaultProjectRole.id)
			.subscribe(() => {
					this.notificationService.success('Project successfully assigned.');
					this.updateAssignedProjects(null, true);
					this.updateNotAssignedProjects(null, true);
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger('Error adding project.');
				}
			);
	}

	assignToPublic(userProject: UserProject, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.assignProjectToUser(userProject.memberId, userProject.projectId, userProject.roleId)
			.finally(() => target.classList.remove('ct-loading'))
			.subscribe(() => {
					this.notificationService.success('Access level has been changed.');
					this.updateAssignedProjects();
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
					this.updateAssignedProjects();
				},
				() => {
					this.notificationService.danger('Access level has not been changed.');
				}
			);
	}

	removeFromUser(userProject: UserProject, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.usersService.removeFromProject(userProject)
			.subscribe(() => {
					this.notificationService.success('User was removed from project.');
					this.updateAssignedProjects(null, true);
					this.updateNotAssignedProjects(null, true);
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger('Error removing project.');
				}
			);
	}

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
