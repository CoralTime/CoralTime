import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Subject } from 'rxjs';
import { PagedResult } from '../../services/odata';
import { Client } from '../../models/client';
import { AclService } from '../../core/auth/acl.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';
import { NotificationService } from '../../core/notification.service';
import { ImpersonationService } from '../../services/impersonation.service';
import { ClientsService } from '../../services/clients.service';
import { ClientFormComponent } from './form/client-form.component';
import { ClientProjectAssignmentComponent } from './project-assignment/project-assignment.component';

@Component({
	selector: 'ct-clients',
	templateUrl: 'clients.component.html'
})

export class ClientsComponent implements OnInit {
	isActiveTab: boolean = true;
	isAllClients: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<Client>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	private lastEvent: any;
	private subject = new Subject<any>();

	private dialogRef: MatDialogRef<ClientFormComponent>;
	private dialogProjectAssignmentRef: MatDialogRef<ClientProjectAssignmentComponent>;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	constructor(private aclService: AclService,
	            private clientsService: ClientsService,
	            private dialog: MatDialog,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		this.getClients();
	}

	// GRID DISPLAYING

	loadLazy(event = null, updatePage?: boolean): void {
		if (event) {
			this.lastEvent = event;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.lastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllClients = false;
			this.pagedResult = null;
			this.resizeObservable.next(true);
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllClients) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	onEndScroll(): void {
		if (!this.isAllClients) {
			this.loadLazy();
		}
	}

	private checkIsAllClients(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllClients = true;
		}
	}

	private getClients(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.clientsService.getClientsWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
		})
			.subscribe((res: PagedResult<Client>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}

					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
					this.checkIsAllClients();
				},
				() => this.notificationService.danger('Error loading clients.')
			);
	}

	// FORM

	openClientDialog(client: Client = null): void {
		if (!this.aclService.isGranted('roleEditClient')) {
			return;
		}

		this.dialogRef = this.dialog.open(ClientFormComponent);
		this.dialogRef.componentInstance.client = client;

		this.dialogRef.componentInstance.onSubmit.subscribe((response) => {
			this.dialogRef.close();
			this.onSubmit(response);
		});
	}

	openProjectAssignmentDialog(client: Client = null): void {
		this.dialogProjectAssignmentRef = this.dialog.open(ClientProjectAssignmentComponent);
		this.dialogProjectAssignmentRef.componentInstance.client = client;
	}

	private onSubmit(response: any): void {
		if (response.error) {
			this.notificationService.danger('Error saving client.');
			return;
		}

		if (response.isNewClient) {
			this.notificationService.success('New client has been successfully created.');
		} else {
			this.notificationService.success('Client has been successfully changed.');
		}

		this.loadLazy(null, true);
	}

	// GENERAL

	onResize(): void {
		this.resizeObservable.next();
	}

	toggleTab(isActiveTab: boolean): void {
		if (this.lastEvent) {
			this.lastEvent.first = 0;
			this.lastEvent.pageCount = 1;
		}

		this.isActiveTab = isActiveTab;
		this.loadLazy(null, true);
	}
}
