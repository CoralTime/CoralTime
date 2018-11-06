import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Client } from '../../../models/client';
import { Project } from '../../../models/project';
import { ROWS_ON_PAGE } from '../../../core/constant.service';
import { NotificationService } from '../../../core/notification.service';
import { PagedResult } from '../../../services/odata';
import { ProjectsService } from '../../../services/projects.service';

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
			return this.projectsService.getClientProjects(this.notAssignedProjectsLastEvent, this.filterStr, true, null);
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
			this.isAllNotAssignedProjects = false;
		}
		if (updatePage) {
			this.updatingNotAssignedProjectsGrid = updatePage;
			this.notAssignedProjectsLastEvent.first = 0;
			this.isAllNotAssignedProjects = false;
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

	updateProjectClient(project: Project, client: Client = null, target: HTMLElement): void {
		let newProjectClient = project;
		if (client) {
			newProjectClient.clientId = client.id;
			newProjectClient.clientName = client.name;
		} else {
			newProjectClient.clientId = null;
			newProjectClient.clientName = null;
		}

		target.classList.add('ct-loading');
		this.projectsService.odata.Put(newProjectClient, newProjectClient.id.toString())
			.subscribe(() => {
					this.updateAssignedProjects(null, true);
					this.updateNotAssignedProjects(null, true);
					this.notificationService.success('Project successfully ' + (client ? 'assigned.' : 'unassigned.'));
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger(client ? 'Error assigning client.' : 'Error deleting project from client');
				});
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
