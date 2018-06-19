import { Component } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';
import { NotificationService } from '../../core/notification.service';

@Component({
	selector: 'ct-admin',
	templateUrl: 'admin.component.html'
})

export class AdminComponent {

	constructor(private adminService: AdminService,
	            private loadingService: LoadingMaskService,
	            private notificationService: NotificationService) {

	}

	updateManagersRoles(): void {
		this.loadingService.addLoading();
		this.adminService.updateManagersRoles()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};

	resetCache(): void {
	this.loadingService.addLoading();
	this.adminService.resetCache()
		.finally(() => this.loadingService.removeLoading())
		.subscribe(() => {
				this.notificationService.success('Done');
			},
			errResponse => {
				this.notificationService.danger('Error');
			});
};

	updateClaims(): void {
		this.loadingService.addLoading();
		this.adminService.updateClaims()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};

	refreshDataBase(): void {
		this.loadingService.addLoading();
		this.adminService.refreshDataBase()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};

	updateAvatars(): void {
		this.loadingService.addLoading();
		this.adminService.updateAvatars()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};

	sendWeeklyUpdates(): void {
		this.loadingService.addLoading();
		this.adminService.sendWeeklyUpdates()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};
}
