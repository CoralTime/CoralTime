
import {finalize} from 'rxjs/operators';
import { Component } from '@angular/core';
import { NotificationService } from '../../core/notification.service';
import { AdminService } from '../../services/admin.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Component({
	selector: 'ct-admin',
	templateUrl: 'admin.component.html'
})

export class AdminComponent {
	filterStr: string = '';

	constructor(private adminService: AdminService,
	            private loadingService: LoadingMaskService,
	            private notificationService: NotificationService) {

	}

	updateManagersRoles(): void {
		this.loadingService.addLoading();
		this.adminService.updateManagersRoles().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};

	resetCache(): void {
		this.loadingService.addLoading();
		this.adminService.resetCache().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};

	updateClaims(): void {
		this.loadingService.addLoading();
		this.adminService.updateClaims().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};

	updateAvatars(): void {
		this.loadingService.addLoading();
		this.adminService.updateAvatars().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};

	sendNotificationFromProject(): void {
		this.loadingService.addLoading();
		this.adminService.sendNotificationFromProject().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
	};

	sendNotificationsWeekly(): void {
		this.loadingService.addLoading();
		this.adminService.sendNotificationsWeekly().pipe(
			finalize(() => this.loadingService.removeLoading()))
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				() => {
					this.notificationService.danger('Error');
				});
    };
}
