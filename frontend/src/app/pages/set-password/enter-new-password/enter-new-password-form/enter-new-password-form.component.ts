import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { SetPasswordService, PasswordChangingStatus } from '../set-password.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../../../core/notification.service';

const ERRORS_PASSWORD_CHANGING = [
	'success',
	'An account doesn\'t exist with this email address, please check that it\'s entered correctly!',
	'Error sending email.',
	'Your token was expired. Please, try again.',
	'Your password is incorrect. Try again or write to admin.',
	'Something went wrong. Please, try again.'
];

@Component({
	selector: 'ct-enter-new-password-form',
	templateUrl: 'enter-new-password-form.component.html'
})

export class EnterNewPasswordFormComponent implements OnInit {
	@Output() public newPasswordSubmitted: EventEmitter<PasswordChangingStatus> = new EventEmitter<PasswordChangingStatus>();

	email: string;
	errorMessage: string;
	password1: string;
	password2: string;
	token: string;

	constructor(private notificationService: NotificationService,
	            private route: ActivatedRoute,
	            private router: Router,
	            private setPasswordService: SetPasswordService) {
	}

	ngOnInit() {
		this.route.queryParams.subscribe((params) => {
			this.token = params['restoreCode'];
		});
	}

	saveNewPassword(token: string, password: string): void {
		this.setPasswordService.saveNewPassword(token, password).then((passwordChangeStatus) => {
			this.errorMessage = null;
			if (!passwordChangeStatus) {
				this.errorMessage = ERRORS_PASSWORD_CHANGING[5];
				return 0;
			}
			if (passwordChangeStatus.isChangedPassword) {
				this.notificationService.success('You have successfully updated your password.');
				this.router.navigate(['/login']);
			} else {
				this.errorMessage = ERRORS_PASSWORD_CHANGING[passwordChangeStatus.message];
			}
		});
	}
}




