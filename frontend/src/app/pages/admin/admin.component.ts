import { Component } from '@angular/core';
import { NotificationService } from '../../core/notification.service';
import { AdminService } from '../../services/admin.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Component({
	selector: 'ct-admin',
	templateUrl: 'admin.component.html'
})

export class AdminComponent {
	memberId: number;
	projectIds: number[];
	filterStr: string = '';

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

	sendNotificationFromProject(): void {
		this.loadingService.addLoading();
		this.adminService.sendNotificationFromProject()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
	};

	sendNotificationsWeekly(): void {
		this.loadingService.addLoading();
		this.adminService.sendNotificationsWeekly()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.notificationService.success('Done');
				},
				errResponse => {
					this.notificationService.danger('Error');
				});
    };

    updateVstsProjects(): void {
        this.loadingService.addLoading();
        this.adminService.updateVstsProjects()
            .finally(() => this.loadingService.removeLoading())
            .subscribe(() => {
                this.notificationService.success('Done');
            },
            errResponse => {
                this.notificationService.danger('Error');
            });
    };

    updateVstsUsers(): void {
        this.loadingService.addLoading();
        this.adminService.updateVstsUsers()
            .finally(() => this.loadingService.removeLoading())
            .subscribe(() => {
                this.notificationService.success('Done');
            },
            errResponse => {
                this.notificationService.danger('Error');
            });
    };
}
