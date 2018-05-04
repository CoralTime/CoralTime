import { Component, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { EnterEmailService, EmailSendingStatus } from '../enter-email.service';
import { SetPasswordService } from '../../enter-new-password/set-password.service';
import { EMAIL_PATTERN } from '../../../../core/constant.service';

const ERRORS_EMAIL_SENDING = [
	'success',
	'Please provide a valid email.',
	'Error sending email. Please, try later.'
];

@Component({
	selector: 'ct-enter-email-form',
	templateUrl: 'enter-email-form.component.html'
})

export class EnterEmailFormComponent implements OnInit, OnDestroy {
	@Output() emailSubmitted: EventEmitter<EmailSendingStatus> = new EventEmitter<EmailSendingStatus>();

	activationCodeIsValid: boolean = true;
	email: string;
	isEmailValid: boolean = true;

	emailPattern = EMAIL_PATTERN;
	errorMessage: string;
	errorCode: number;

	constructor(private enterEmailService: EnterEmailService,
	            private setPasswordService: SetPasswordService) {
	}

	ngOnInit() {
		this.activationCodeIsValid = !this.setPasswordService.restoreCodeIsExpired;
	}

	sendEmail(): void {
		this.forgotPassValidate();

		if (!this.email) {
			this.errorMessage = 'A valid email address is required.';
		} else if (this.isEmailValid) {
			this.enterEmailService.sendEmail(this.email).subscribe((emailResponse) => {
				this.errorMessage = null;

				if (emailResponse.isSentEmail) {
					this.emailSubmitted.emit(emailResponse);
				} else {
					this.errorCode = emailResponse.message;
					this.errorMessage = ERRORS_EMAIL_SENDING[this.errorCode];

					if (this.errorCode === 1) {
						this.errorMessage = 'This email isn\'t in our system. Make sure you typed your address correctly, or contact support.';
					}

					if (this.errorCode === 5) {
						this.errorMessage = 'You can\'t log in to this account. Please contact support.';
					}
				}

				this.activationCodeIsValid = true;
				this.setPasswordService.restoreCodeIsExpired = false;
			});
		}
	}

	forgotPassValidate(): void {
		if (this.email) {
			this.isEmailValid = this.validateEmail(this.email);
		}
	}

	validateEmail(email): boolean {
		return EMAIL_PATTERN.test(email);
	}

	ngOnDestroy() {
		this.activationCodeIsValid = true;
		this.setPasswordService.restoreCodeIsExpired = false;
	}
}
