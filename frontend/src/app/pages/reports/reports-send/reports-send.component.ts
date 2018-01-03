import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ClientsService } from '../../../services/clients.service';
import { Client } from '../../../models/client';
import { ReportsService } from '../../../services/reposts.service';
import { NgForm } from '@angular/forms';
import { User } from '../../../models/user';
import * as moment from 'moment';
import { EMAIL_PATTERN } from '../../../core/constant.service';

export class SendReportsFormModel {
	bccEmails: string[];
	ccEmails: string[];
	clientIds: number[];
	comment: string;
	dateFormatId: number;
	dateFrom: string;
	dateTo: string;
	fileTypeId: number;
	fromEmail: string;
	groupById: number;
	memberIds: number[];
	projectIds: number[];
	showColumnIds: number;
	subject: string;
	toEmail: string;

	constructor(data: any = null) {
		if (data) {
			this.bccEmails = data.bccEmails || [];
			this.ccEmails = data.ccEmails || [];
			this.clientIds = data.clientIds;
			this.comment = data.comment;
			this.dateFormatId = data.dateFormatId;
			this.dateFrom = data.dateFrom;
			this.dateTo = data.dateTo;
			this.fileTypeId = data.fileTypeId;
			this.fromEmail = data.fromEmail;
			this.groupById = data.groupById;
			this.memberIds = data.memberIds;
			this.projectIds = data.projectIds;
			this.showColumnIds = data.showColumnIds;
			this.subject = data.subject;
			this.toEmail = data.toEmail;
		}
	}
}

export interface ExportFile {
	label: string;
	fileTypeId: number;
}

const EXPORT_FILE_LIST: ExportFile[] = [
	{
		label: 'Excel',
		fileTypeId: 0
	},
	{
		label: 'CSV',
		fileTypeId: 1
	},
	{
		label: 'PDF',
		fileTypeId: 2
	},
];

@Component({
	selector: 'ct-reports-send',
	templateUrl: 'reports-send.component.html'
})

export class ReportsSendComponent implements OnInit {
	model: SendReportsFormModel;
	clients: Client[];
	clientModel: Client;
	dateFormat: string;
	userInfo: User;

	emailFromError: string;
	emailPattern = EMAIL_PATTERN;
	isCcFormValid: boolean = true;
	isBccFormValid: boolean = true;
	isFormErrorsShown: boolean = false;
	isRequestLoading: boolean = false;
	exportFileList: ExportFile[] = EXPORT_FILE_LIST;
	exportFileModel: ExportFile = this.exportFileList[0];

	@Output() onSubmit = new EventEmitter();

	constructor(private clientsService: ClientsService,
	            private reportsService: ReportsService) {
	}

	ngOnInit() {
		this.clientsService.getClients().subscribe((clients) => {
			this.clients = clients.filter((client: Client) => !!client.email === true);
			if(this.model.clientIds && this.model.clientIds.length == 1) {
				let selectedClient = clients.filter((client: Client) => client.id === this.model.clientIds[0])[0];
				this.model.toEmail = selectedClient.email;
			}
		});
		this.model.fromEmail = this.userInfo.email;
		this.model.subject = 'CoralTime: ' + this.formatDate(this.model.dateFrom) + ' - ' + this.formatDate(this.model.dateTo);
	}

	checkIsFromEmailEmpty(): void {
		this.emailFromError = null;

		if (!this.model.fromEmail) {
			this.emailFromError = 'Client sender email is required.';
			return
		}
	}

	showErrors(): void {
		this.isFormErrorsShown = true;
		setTimeout(() => this.isFormErrorsShown = false, 3000);
	}

	submit(form: NgForm): void {
		if (form.invalid || !this.isCcFormValid || !this.isBccFormValid) {
			this.checkIsFromEmailEmpty();
			this.showErrors();
			return;
		}

		this.isRequestLoading = true;
		this.reportsService.sendReports(this.model).subscribe((res) => {
			this.isRequestLoading = false;
			this.onSubmit.emit(res);
		})
	}

	private formatDate(utcDate: string): string {
		if (!utcDate) {
			return;
		}
		let date = moment(utcDate);

		return this.dateFormat ? date.format(this.dateFormat) : date.toDate().toLocaleDateString();
	}
}