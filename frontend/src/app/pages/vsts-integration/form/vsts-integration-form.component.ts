import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { VstsProjectConnection } from '../../../models/vsts-project-connection';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';
import { ProjectsService } from '../../../services/projects.service';
import { VstsIntegrationService } from '../../../services/vsts-integration.service';
import { Project } from '../../../models/project';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';

export class FormConnection {
	id: number;
	projectId: number;
	projectName: string;
	vstsProjectId: number;
	vstsProjectName: string;
	vstsCompanyUrl: string;
	vstsPat: string;

	static formConnection(connection: VstsProjectConnection) {
		let instance = new this;
		instance.id = connection.id;
		instance.projectId = connection.projectId;
		instance.projectName = connection.projectName;
		instance.vstsProjectId = connection.vstsProjectId;
		instance.vstsProjectName = connection.vstsProjectName;
		instance.vstsCompanyUrl = connection.vstsCompanyUrl;
		instance.vstsPat = connection.vstsPat;

		return instance;
	}

	toConnection(connection: VstsProjectConnection, isPatChanged: boolean = false) {
		connection.id = this.id;
		connection.projectId = this.projectId;
		connection.projectName = this.projectName;
		connection.vstsProjectId = this.vstsProjectId;
		connection.vstsProjectName = this.vstsProjectName;
		connection.vstsCompanyUrl = this.vstsCompanyUrl;
		connection.vstsPat = this.id && !isPatChanged ? null : this.vstsPat;

		return connection;
	}
}

@Component({
	selector: 'ct-vsts-integration-form',
	templateUrl: 'vsts-integration-form.component.html'
})

export class VstsIntegrationFormComponent implements OnInit {
	@Input() connection: VstsProjectConnection;
	@Output() onSubmit = new EventEmitter();

	projects: Project[];
	projectModel: Project;
	defaultProjectName: string;
	dialogHeader: string;
	isProjectSelectDisabled: boolean;
	isProjectsLoading: boolean;
	isNewConnection: boolean;
	isRequestLoading: boolean;
	isValidateLoading: boolean;
	model: FormConnection;
	patPattern = /^[a-zA-Z0-9]+$/;
	showErrors: boolean[] = []; // [showCoraltimeProjectError, showVstsProjectError, showCompanyUrlError, showVstsPatError]
	submitButtonText: string;

	constructor(private loadingService: LoadingMaskService,
	            private projectsService: ProjectsService,
	            private vstsIntegrationService: VstsIntegrationService) {
	}

	ngOnInit() {
		this.getProjects();

		let connection = this.connection;
		this.isNewConnection = !connection;
		this.connection = connection ? connection : new VstsProjectConnection();
		this.submitButtonText = this.connection.id ? 'Save' : 'Create';
		this.dialogHeader = this.connection.id ? 'Edit' : 'Create New Connection';
		this.model = FormConnection.formConnection(this.connection);
	}

	getProjects(): void {
		this.isProjectsLoading = true;
		this.projectsService.getProjects()
			.finally(() => this.isProjectsLoading = false)
			.subscribe((projects) => {
				this.projects = projects;
				this.projectModel = this.projects.filter((project) => project.id === this.connection.projectId)[0];
			});
	}

	projectOnChange(): void {
		this.showErrors[0] = false;
		this.model.projectId = this.projectModel.id;
		this.model.projectName = this.projectModel.name;
	}

	vstsProjectNameOnChange(): void {
		this.showErrors[1] = false;
		this.model.vstsProjectId = null;
	}

	validateAndSubmit(form: NgForm): void {
		this.isValidateLoading = true;
		this.validateForm(form)
			.finally(() => this.isValidateLoading = false)
			.subscribe((isFormValid: boolean) => {
				if (isFormValid) {
					this.submit(form);
				}
			});
	}

	private submit(form: NgForm): void {
		const isVstsPatChanged: boolean = !form.controls['vstsPat'].pristine;
		let submitObservable: Observable<any>;

		this.connection = this.model.toConnection(this.connection, isVstsPatChanged);

		if (this.connection.id) {
			submitObservable = this.vstsIntegrationService.odata.Put(this.connection, this.connection.id.toString());
		} else {
			submitObservable = this.vstsIntegrationService.odata.Post(this.connection);
		}

		this.isRequestLoading = true;
		this.loadingService.addLoading();
		submitObservable.finally(() => {
			this.isRequestLoading = false;
			this.loadingService.removeLoading();
		})
			.subscribe(() => {
					this.onSubmit.emit({
						isNewConnection: this.isNewConnection
					});
				},
				error => this.onSubmit.emit({
					isNewConnection: this.isNewConnection,
					error: error
				}));
	}

	private validateForm(form: NgForm): Observable<boolean> {
		this.showErrors = [false, false, false, false];

		const isCoralTimeProjectObservable = Observable.of(form.controls['project'].valid);
		const isCompanyUrlValidObservable = Observable.of(form.controls['vstsCompanyUrl'].valid);
		const isVstsPatChanged: boolean = !form.controls['vstsPat'].pristine;

		let isVstsProjectValidObservable: Observable<any>;
		let isVstsPatValidObservable = Observable.of(form.controls['vstsPat'].valid);

		if (!this.model.vstsProjectName || !this.model.vstsProjectName.trim()) {
			isVstsProjectValidObservable = Observable.of(false);
		} else {
			isVstsProjectValidObservable = this.vstsIntegrationService.getConnectionsByProjectName(this.model.vstsProjectName)
				.map((connection) => !connection || (connection.id === this.model.id));
		}

		if (this.model.id && !isVstsPatChanged) {
			isVstsPatValidObservable = Observable.of(true);
		} else {
			isVstsPatValidObservable = Observable.of(form.controls['vstsPat'].valid);
		}

		return Observable.forkJoin(
			isCoralTimeProjectObservable,
			isVstsProjectValidObservable,
			isCompanyUrlValidObservable,
			isVstsPatValidObservable
		).map((response: boolean[]) => {
				console.log(response);
				return response.map((isControlValid, i) => this.showErrors[i] = !isControlValid)
					.every((showError) => showError === false)
			}
		);
	}
}
