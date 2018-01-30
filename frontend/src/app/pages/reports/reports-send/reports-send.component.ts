import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ClientsService } from '../../../services/clients.service';
import { Client } from '../../../models/client';
import { ReportFilters, ReportsService } from '../../../services/reposts.service';
import { NgForm } from '@angular/forms';
import { User } from '../../../models/user';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import * as moment from 'moment';

export class SendReportsFormModel {
	bccEmails: string[];
	ccEmails: string[];
	comment: string;
	dateFormatId: number;
	fileTypeId: number;
	fromEmail: string;
	subject: string;
	toEmail: string;
	valuesSaved: ReportFilters;

	constructor(data: any = null) {
		if (data) {
			this.bccEmails = data.bccEmails || [];
			this.ccEmails = data.ccEmails || [];
			this.comment = data.comment;
			this.dateFormatId = data.dateFormatId;
			this.fileTypeId = data.fileTypeId;
			this.fromEmail = data.fromEmail;
			this.subject = data.subject;
			this.toEmail = data.toEmail;
			this.valuesSaved = data.defaultQuery;
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
	@Input() model: SendReportsFormModel;
	@Input() dateFormat: string;
	@Input() projectName: string;
	@Input() userInfo: User;

	clients: Client[];
	clientModel: Client;
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
			if (this.model.valuesSaved.clientIds && this.model.valuesSaved.clientIds.length === 1) {
				let selectedClient = clients.filter((client: Client) => client.id === this.model.valuesSaved.clientIds[0])[0];
				this.model.toEmail = selectedClient.email;
			}
		});
		this.model.fromEmail = this.userInfo.email;
		let addProjectName = this.projectName ? this.projectName + ': ' : '';
		this.model.subject = 'CoralTime: ' + addProjectName + this.formatDate(this.model.valuesSaved.dateFrom)
			+ ' - ' + this.formatDate(this.model.valuesSaved.dateTo);
	}

	showErrors(): void {
		this.isFormErrorsShown = true;
		setTimeout(() => this.isFormErrorsShown = false, 3000);
	}

	submit(form: NgForm): void {
		if (form.invalid || !this.isCcFormValid || !this.isBccFormValid) {
			this.showErrors();
			return;
		}

		this.isRequestLoading = true;
		this.reportsService.sendReports(this.model).subscribe((res) => {
			this.isRequestLoading = false;
			this.onSubmit.emit(res);
		});
	}

	private formatDate(utcDate: Date | string): string {
		if (!utcDate) {
			return;
		}
		let date = moment(utcDate);

		return this.dateFormat ? date.format(this.dateFormat) : date.toDate().toLocaleDateString();
	}
}
