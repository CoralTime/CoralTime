import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Subject } from 'rxjs';
import { PagedResult } from '../../services/odata';
import { VstsProjectConnection } from '../../models/vsts-project-connection';
import { ROWS_ON_PAGE } from '../../core/constant.service';
import { NotificationService } from '../../core/notification.service';
import { VstsIntegrationService } from '../../services/vsts-integration.service';
import { VstsIntegrationFormComponent } from './form/vsts-integration-form.component';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';
import { ProjectUsersFormComponent } from './project-users-form/project-users-form.component';

@Component({
	selector: 'ct-vsts-integration',
	templateUrl: 'vsts-integration.component.html'
})

export class VstsIntegrationComponent implements OnInit {
	isAllConnections: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<VstsProjectConnection>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private subject = new Subject<any>();
	private lastEvent: any;

	private dialogRef: MatDialogRef<VstsIntegrationFormComponent>;
	private dialogProjectAssignmentRef: MatDialogRef<ProjectUsersFormComponent>;

	constructor(private dialog: MatDialog,
	            private loadingService: LoadingMaskService,
	            private notificationService: NotificationService,
	            private vstsIntegrationService: VstsIntegrationService
	) {
	}

	ngOnInit() {
		this.getConnections();
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
			this.isAllConnections = false;
			this.pagedResult = null;
			this.resizeObservable.next(true);
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllConnections) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	onEndScroll(): void {
		if (!this.isAllConnections) {
			this.loadLazy();
		}
	}

	removeConnection(connection: VstsProjectConnection, target: HTMLElement): void {
		target.classList.add('ct-loading');
		this.vstsIntegrationService.odata.Delete(connection.id + '')
			.subscribe(() => {
					this.notificationService.success('Connection was deleted.');
					this.loadLazy(null, true);
				},
				() => {
					target.classList.remove('ct-loading');
					this.notificationService.danger('Error deleting connection.');
				}
			);
	}

	private checkIsAllUsers(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllConnections = true;
		}
	}

	private getConnections(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.vstsIntegrationService.getConnectionsWithCount(this.lastEvent, this.filterStr);
		})
			.subscribe((res: PagedResult<VstsProjectConnection>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}

					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
					this.checkIsAllUsers();
				},
				() => this.notificationService.danger('Error loading Connections.')
			);
	}

	// FORM

	openConnectionDialog(connection: VstsProjectConnection = null): void {
		this.dialogRef = this.dialog.open(VstsIntegrationFormComponent);
		this.dialogRef.componentInstance.connection = connection;
		this.dialogRef.componentInstance.onSubmit.subscribe((response) => {
			this.dialogRef.close();
			this.onSubmit(response);
		});
	}

	openProjectAssignmentDialog(connection: VstsProjectConnection = null): void {
		this.dialogProjectAssignmentRef = this.dialog.open(ProjectUsersFormComponent);
		this.dialogProjectAssignmentRef.componentInstance.connection = connection;
	}

	private onSubmit(response: any): void {
		if (response.error) {
			this.notificationService.danger('Error saving connection.');
			return;
		}

		if (response.isNewConnection) {
			this.notificationService.success('New connection has been successfully created.');
			this.loadLazy(null, true);
		} else {
			this.notificationService.success('Connection has been successfully changed.');
			this.loadLazy(null);
		}
	}

	// GENERAL

	formatUrlForMarkdown(url: string): string {
		return '<' + url + '>';
	}

	onResize(): void {
		this.resizeObservable.next();
	}

	updateVstsUsers(): void {
		this.loadingService.addLoading();
		this.vstsIntegrationService.updateVstsUsers()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};
}
