import { Observable } from 'rxjs/Observable';
import { ClientsService } from '../../../services/clients.service';
import { Client } from '../../../models/client';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ArrayUtils } from '../../../core/object-utils';
import { NgForm } from '@angular/forms';
import { EMAIL_PATTERN } from '../../../core/constant.service';

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
	client: Client;
	dialogHeader: string;
	submitButtonText: string;
	errorMessage: string;

	isActive: boolean;
	isRequestLoading: boolean = false;
	emailPattern = EMAIL_PATTERN;
	stateModel: any;
	stateText: string;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'archived'}
	];

	model: FormClient;

	@Output() onSubmit = new EventEmitter();

	private isNewClient: boolean;

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

	checkIsNameEmpty(): void {
		this.errorMessage = null;

		if (!this.model.name) {
			this.errorMessage = 'Client name is required.';
			return;
		}
	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'All projects assigned to the archived client are not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	submit(form: NgForm): void {
		this.checkIsNameEmpty();
		if (form.invalid) {
			return;
		}

		this.client = this.model.toClient(this.client);
		let submitObservable: Observable<any>;

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
}
