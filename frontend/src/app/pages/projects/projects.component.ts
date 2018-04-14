import { Subject } from 'rxjs/Subject';
import { ProjectSettingsFormComponent } from './project-settings-form/project-settings-form.component';
import { ProjectFormComponent } from './project-form/project-form.component';
import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Project } from '../../models/project';
import { ProjectsService } from '../../services/projects.service';
import { NotificationService } from '../../core/notification.service';
import { PagedResult } from '../../services/odata/query';
import { ProjectTasksComponent } from './project-tasks-form/project-tasks.component';
import { ProjectUsersComponent } from './project-members-form/project-members.component';
import { AuthUser } from '../../core/auth/auth-user';
import { AuthService } from '../../core/auth/auth.service';
import { Router } from '@angular/router';
import { ImpersonationService } from '../../services/impersonation.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';

@Component({
	selector: 'ct-projects',
	templateUrl: 'projects.component.html'
})

export class ProjectsComponent implements OnInit {
	isActiveTab: boolean = true;
	isAllProjects: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<Project>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private authUser: AuthUser;
	private lastEvent: any;
	private subject = new Subject<any>();

	private dialogRef: MatDialogRef<ProjectFormComponent>;
	private dialogTasksRef: MatDialogRef<ProjectTasksComponent>;
	private dialogUserRef: MatDialogRef<ProjectUsersComponent>;
	private dialogSettingsRef: MatDialogRef<ProjectSettingsFormComponent>;

	constructor(private authService: AuthService,
	            private dialog: MatDialog,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private projectsService: ProjectsService,
	            private router: Router) {
		this.impersonationService.checkImpersonationRole('projects');
	}

	ngOnInit() {
		this.authUser = this.authService.authUser;
		// TODO: fix string to bool
		if (this.authUser.role !== 1 && this.authUser.isManager !== 'true') {
			this.router.navigate(['/calendar']);
		}
		this.getProjects();
	}

	onEndScroll(): void {
		this.checkIsAllUnassignedProjects();

		if (!this.isAllProjects) {
			this.loadLazy();
		}
	}

	getProjects(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.projectsService.getManagerProjectsWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
		})
			.subscribe(
				(res: PagedResult<Project>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}
					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
				},
				error => this.notificationService.danger('Error loading projects.')
			);
	}

	loadLazy(event = null, updatePage?: boolean): void {
		this.checkIsAllUnassignedProjects();

		if (event) {
			this.lastEvent = event;
			this.isAllProjects = false;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.lastEvent.first = 0;
			this.isAllProjects = false;
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

	private checkIsAllUnassignedProjects(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllProjects = true;
		}
	}

	openProjectDialog(project: Project = null): void {
		this.dialogRef = this.dialog.open(ProjectFormComponent);
		this.dialogRef.componentInstance.project = project;

		this.dialogRef.componentInstance.onSubmit.subscribe((response) => {
			this.dialogRef.close();
			this.onSubmit(response);
		});
	}

	openProjectSettingsDialog(project: Project): void {
		this.dialogSettingsRef = this.dialog.open(ProjectSettingsFormComponent);
		this.dialogSettingsRef.componentInstance.project = project;

		this.dialogSettingsRef.componentInstance.onSaved.subscribe(() => {
			this.dialogSettingsRef.close();
			this.notificationService.success('Project settings has been successfully changed.');
			this.loadLazy(null, true);
		});
	}

	openProjectTasksDialog(project: Project): void {
		this.dialogTasksRef = this.dialog.open(ProjectTasksComponent);
		this.dialogTasksRef.componentInstance.project = project;
	}

	openProjectUsersDialog(project: Project): void {
		this.dialogUserRef = this.dialog.open(ProjectUsersComponent);
		this.dialogUserRef.componentInstance.project = project;
	}

	onSubmit(response: any): void {
		if (response.error) {
			this.notificationService.danger('Error saving project.');
			return;
		}

		if (response.isNewProject) {
			this.notificationService.success('New project has been successfully created.');
		} else {
			this.notificationService.success('Project has been successfully changed.');
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
