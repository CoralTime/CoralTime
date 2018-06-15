import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable } from 'rxjs/Observable';
import { ArrayUtils } from '../../../core/object-utils';
import { Project } from '../../../models/project';
import { Client } from '../../../models/client';
import { ClientsService } from '../../../services/clients.service';
import { ProjectsService } from '../../../services/projects.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';
import { hexToNumber, numberToHex } from '../../../shared/form/color-picker/color-picker.component';

export class FormProject {
	id: number;
	name: string;
	clientId: number;
	clientName: string;
	clientIsActive: boolean;
	color: string;
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
		instance.color = numberToHex(project.color, true);
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
		project.color = hexToNumber(this.color);
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
	@Input() project: Project;
	@Output() onSubmit = new EventEmitter();

	clients: Client[];
	clientModel: Client;
	defaultClientName: string;
	dialogHeader: string;
	isClientSelectDisabled: boolean;
	isClientsLoading: boolean;
	isNewProject: boolean;
	isRequestLoading: boolean;
	isValidateLoading: boolean;
	model: FormProject;
	showErrors: boolean[] = []; // [showColorError, showNameError]
	showNameError: boolean;
	stateModel: any;
	stateText: string;
	submitButtonText: string;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'archived'}
	];

	constructor(private clientsService: ClientsService,
	            private loadingService: LoadingMaskService,
	            private projectsService: ProjectsService,
	            private translatePipe: TranslatePipe) {
	}

	ngOnInit() {
		this.getClients();

		let project = this.project;
		this.isNewProject = !project;
		this.project = project ? project : new Project();
		this.submitButtonText = this.project.id ? 'Save' : 'Create';
		this.dialogHeader = this.project.id ? 'Edit' : this.translatePipe.transform('Create New Project');
		this.model = FormProject.formProject(this.project);
		this.stateModel = ArrayUtils.findByProperty(this.states, 'value', this.model.isActive);
		this.stateText = this.project.isActive ? '' : 'Archived project is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	getClients(): void {
		this.isClientsLoading = true;
		this.clientsService.getClients()
			.finally(() => this.isClientsLoading = false)
			.subscribe((clients) => {
				this.clients = clients;
				this.clientModel = this.clients.filter((client) => client.id === this.project.clientId)[0];

				if (this.model.clientName && !this.clientModel) {
					this.defaultClientName = this.model.clientName + ' (archived)';
					this.isClientSelectDisabled = true;
				}
			});
	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'Archived project is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	clientOnChange(): void {
		this.model.clientIsActive = true;
		this.model.clientId = this.clientModel.id;
	}

	validateAndSubmit(form: NgForm): void {
		this.isValidateLoading = true;
		this.validateForm(form)
			.finally(() => this.isValidateLoading = false)
			.subscribe((isFormValid: boolean) => {
				if (isFormValid) {
					this.submit();
				}
			});
	}

	private submit(): void {
		let submitObservable: Observable<any>;
		this.project = this.model.toProject(this.project);

		if (this.project.id) {
			submitObservable = this.projectsService.odata.Put(this.project, this.project.id.toString());
		} else {
			submitObservable = this.projectsService.odata.Post(this.project);
		}

		this.isRequestLoading = true;
		this.loadingService.addLoading();
		submitObservable.finally(() => {
			this.isRequestLoading = false;
			this.loadingService.removeLoading();
		})
			.subscribe(() => {
					this.onSubmit.emit({
						isNewProject: this.isNewProject
					});
				},
				error => this.onSubmit.emit({
					isNewProject: this.isNewProject,
					error: error
				}));
	}

	private validateForm(form: NgForm): Observable<boolean> {
		this.showErrors = [false, false];

		let isColorValidObservable = Observable.of(form.controls['color'].valid);
		let isNameValidObservable: Observable<any>;

		if (!this.model.name.trim()) {
			isNameValidObservable = Observable.of(false);
		} else {
			isNameValidObservable = this.projectsService.getProjectByName(this.model.name)
				.map((project) => !project || (project.id === this.model.id));
		}

		return Observable.forkJoin(isColorValidObservable, isNameValidObservable)
			.map((response: boolean[]) =>
				response.map((isControlValid, i) => this.showErrors[i] = !isControlValid)
					.every((showError) => showError === false)
			);
	}
}
