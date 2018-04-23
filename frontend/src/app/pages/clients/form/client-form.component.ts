import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable } from 'rxjs/Observable';
import { Client } from '../../../models/client';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import { ArrayUtils } from '../../../core/object-utils';
import { ClientsService } from '../../../services/clients.service';

class FormClient {
	id: number;
	name: string;
	email: string;
	description: string;
	isActive: boolean;

	static formClient(client: Client) {
		let instance = new this;
		instance.id = client.id;
		instance.name = client.name;
		instance.email = client.email;
		instance.description = client.description;
		instance.isActive = client.id ? client.isActive : true;

		return instance;
	}

	toClient(client: Client) {
		client.id = this.id;
		client.name = this.name;
		client.email = this.email;
		client.description = this.description;
		client.isActive = this.isActive;

		return client;
	}
}

@Component({
	selector: 'ct-client-form',
	templateUrl: 'client-form.component.html',
	providers: [TranslatePipe]
})

export class ClientFormComponent implements OnInit {
	@Input() client: Client;
	@Output() onSubmit = new EventEmitter();

	dialogHeader: string;
	isNewClient: boolean;
	isRequestLoading: boolean;
	emailPattern = EMAIL_PATTERN;
	model: FormClient;
	showErrors: boolean[] = []; // [showEmailError, showNameError]
	showNameError: boolean;
	stateModel: any;
	stateText: string;
	submitButtonText: string;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'archived'}
	];

	constructor(private clientsService: ClientsService,
	            private translatePipe: TranslatePipe) {
	}

	ngOnInit() {
		let client = this.client;
		this.isNewClient = !client;
		this.client = client ? client : new Client();
		this.submitButtonText = this.client.id ? 'Save' : 'Create';
		this.dialogHeader = this.client.id ? 'Edit' : this.translatePipe.transform('Create New Client');
		this.model = FormClient.formClient(this.client);
		this.stateModel = ArrayUtils.findByProperty(this.states, 'value', this.model.isActive);
		this.stateText = this.client.isActive ? '' : 'All projects assigned to the archived client are not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'All projects assigned to the archived client are not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	validateAndSubmit(form: NgForm): void {
		this.isRequestLoading = true;
		this.validateForm(form)
			.subscribe((isFormValid: boolean) => {
					this.isRequestLoading = false;
					if (isFormValid) {
						this.submit();
					}
				},
				() => this.isRequestLoading = false);
	}

	private submit(): void {
		let submitObservable: Observable<any>;
		this.client = this.model.toClient(this.client);

		if (this.client.id) {
			submitObservable = this.clientsService.odata.Put(this.client, this.client.id.toString());
		} else {
			submitObservable = this.clientsService.odata.Post(this.client);
		}

		this.isRequestLoading = true;
		submitObservable.toPromise().then(
			() => {
				this.isRequestLoading = false;
				this.onSubmit.emit({
					isNewClient: this.isNewClient
				});
			},
			error => this.onSubmit.emit({
				isNewClient: this.isNewClient,
				error: error
			})
		);
	}

	private validateForm(form: NgForm): Observable<boolean> {
		this.showErrors = [false, false];

		let isEmailValidObservable = Observable.of(!form.controls['email'].errors);
		let isNameValidObservable: Observable<any>;

		if (!this.model.name) {
			isNameValidObservable = Observable.of(false);
		} else {
			isNameValidObservable = this.clientsService.getClientByName(this.model.name)
				.map((client) => !client || (client.id === this.model.id));
		}

		return Observable.forkJoin(isEmailValidObservable, isNameValidObservable)
			.map((response: boolean[]) =>
				response.map((isControlValid, i) => this.showErrors[i] = !isControlValid)
					.every((showError) => showError === false)
			);
	}
}
