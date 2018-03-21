import { NotificationService } from '../../../core/notification.service';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { PagedResult } from '../../../services/odata/query';
import { Subject } from 'rxjs/Subject';
import { Project } from '../../../models/project';
import { Client } from '../../../models/client';
import { ProjectsService } from '../../../services/projects.service';
import { Observable } from 'rxjs/Observable';
import { ROWS_ON_PAGE } from '../../../core/constant.service';

@Component({
	selector: 'ct-client-project-assignment',
	templateUrl: 'project-assignment.component.html'
})

export class ClientProjectAssignmentComponent implements OnInit {
	@Input() client: Client;
	@ViewChild('grid') gridContainer: ElementRef;

	filterStr: string = '';
	resizeObservable: Subject<any> = new Subject();
	wrapperHeightObservable: Subject<any> = new Subject();

	assignedProjectsPagedResult: PagedResult<Project>;
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
	            private projectsService: ProjectsService) {
	}

	ngOnInit() {
		this.loadAssignedProjects();
		this.loadNotAssignedProjects();

		this.wrapperHeightObservable.debounceTime(100).subscribe(() => {
			this.changeScrollableContainer();
			this.resizeObservable.next();
		});
	}

	// ASSIGNED PROJECTS GRID

	loadAssignedProjects(): void {
		this.assignedProjectsSubject.debounceTime(500).switchMap(() => {
			return this.projectsService.getClientProjects(this.assignedProjectsLastEvent, this.filterStr, this.client.isActive, this.client.id);
		})
			.subscribe(
				(res: PagedResult<Project>) => {
					if (!this.assignedProjectsPagedResult || !this.assignedProjectsLastEvent.first || this.updatingAssignedProjectsGrid) {
						this.assignedProjectsPagedResult = res;
					} else {
						this.assignedProjectsPagedResult.data = this.assignedProjectsPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.client.projectsCount = this.assignedProjectsPagedResult.count;
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
			return this.projectsService.getClientProjects(this.notAssignedProjectsLastEvent, this.filterStr, true, null);
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

	updateProjectClient(project: Project, client: Client = null): void {
		let newProjectClient = project;
		if (client) {
			newProjectClient.clientId = client.id;
			newProjectClient.clientName = client.name;
		} else {
			newProjectClient.clientId = null;
			newProjectClient.clientName = null;
		}

		let submitObservable: Observable<any>;
		submitObservable = this.projectsService.odata.Put(newProjectClient, newProjectClient.id.toString());
		submitObservable.toPromise()
			.then(() => {
				this.updateAssignedProjects(null, true);
				this.updateNotAssignedProjects(null, true);
				this.filterStr = '';
				this.notificationService.success('Project successfully ' + (client ? 'assigned.' : 'unassigned.'));
			})
			.catch(() => {
				this.notificationService.danger(client ? 'Error assigning client.' : 'Error deleting project from client');
			});
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
