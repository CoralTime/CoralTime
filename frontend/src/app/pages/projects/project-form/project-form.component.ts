import { ClientsService } from '../../../services/clients.service';
import { Observable } from 'rxjs/Observable';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

import { ProjectsService } from '../../../services/projects.service';
import { Project } from '../../../models/project';
import { Client } from '../../../models/client';
import { ArrayUtils } from '../../../core/object-utils';

export class FormProject {
	id: number;
	name: string;
	clientId: number;
	clientName: string;
	clientIsActive: boolean;
	color: number;
	description: string;
	isActive: boolean;
	isPrivate: boolean;

	static formProject(project: Project) {
		let instance = new this;
		instance.id = project.id;
		instance.name = project.name;
		instance.clientId = project.clientId;
		instance.clientName = project.clientName;
		instance.clientIsActive = project.clientIsActive;
		instance.color = project.color ? project.color : 0;
		instance.description = project.description;
		instance.isActive = project.id ? project.isActive : true;
		instance.isPrivate = project.isPrivate;

		return instance;
	}

	toProject(project: Project) {
		project.id = this.id;
		project.name = this.name;
		project.clientId = this.clientId;
		project.clientName = this.clientName;
		project.clientIsActive = this.clientIsActive;
		project.color = this.color;
		project.description = this.description;
		project.isActive = this.isActive;
		project.isPrivate = this.isPrivate;

		return project;
	}
}

@Component({
	selector: 'ct-project-form',
	templateUrl: 'project-form.component.html',
	providers: [TranslatePipe]
})

export class ProjectFormComponent implements OnInit {
	clients: Client[];
	dialogHeader: string;
	project: Project;
	submitButtonText: string;

	clientModel: Client;
	defaultClientName: string;
	isClientSelectDisabled: boolean = false;
	isRequestLoading: boolean = false;
	model: FormProject;

	isActive: boolean;
	stateModel: any;
	stateText: string;
	errorMessage: string;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'archived'}
	];

	@Output() onSubmit = new EventEmitter();
	private isNewProject: boolean;

	constructor(private clientsService: ClientsService,
	            private projectsService: ProjectsService,
	            private translatePipe: TranslatePipe) {
	}

	ngOnInit() {
		this.clientsService.getClients().subscribe((clients) => {
			this.clients = clients;
			this.clientModel = this.clients.filter((client) => client.id === this.project.clientId)[0];

			if (this.model.clientName && !this.clientModel) {
				this.defaultClientName = this.model.clientName + ' (archived)';
				this.isClientSelectDisabled = true;
			}
		});
		let project = this.project;

		this.isNewProject = !project;
		this.project = project ? project : new Project();
		this.submitButtonText = this.project.id ? 'Save' : 'Create';
		this.dialogHeader = this.project.id ? 'Edit' : this.translatePipe.transform('Create New Project');
		this.model = FormProject.formProject(this.project);
		this.stateModel = ArrayUtils.findByProperty(this.states, 'value', this.model.isActive);

		this.stateText = this.project.isActive ? '' : 'Archived project is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'Archived project is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	clientOnChange(): void {
		this.model.clientIsActive = true;
		this.model.clientId = this.clientModel.id;
	}

	submit(): void {
		this.errorMessage = null;
		this.project = this.model.toProject(this.project);

		if (!this.project.name) {
			this.errorMessage = 'Project name is required.';
		} else {
			let submitObservable: Observable<any>;

			if (this.project.id) {
				submitObservable = this.projectsService.odata.Put(this.project, this.project.id.toString());
			} else {
				submitObservable = this.projectsService.odata.Post(this.project);
			}

			this.isRequestLoading = true;
			submitObservable.toPromise().then(
				() => {
					this.isRequestLoading = false;
					this.onSubmit.emit({
						isNewTask: this.isNewProject
					});
				},
				error => this.onSubmit.emit({
					isNewTask: this.isNewProject,
					error: error
				}));
		}
	}
}
