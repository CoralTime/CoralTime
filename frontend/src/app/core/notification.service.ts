import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material';

@Injectable()
export class NotificationService {
	private config: MatSnackBarConfig;
	private canShowNotification: boolean = true;

	constructor(private snackBar: MatSnackBar) {
		this.config = new MatSnackBarConfig();
		this.config.duration = 3000;
	}

	success(message: string): void {
		this.notify(message, 'success');
	}

	danger(message: string): void {
		if (this.canShowNotification) {
			this.notify(message, 'danger');
			this.canShowNotification = false;
		}
		setTimeout(() => { this.canShowNotification = true; }, 1000);
	}

	private notify(message: string, type: string): void {
		let config = Object.assign({}, this.config);
		config.extraClasses = ['snack-bar-' + type];
		this.snackBar.open(message, null, config);
	}
}
