import { Component } from '@angular/core';
import { EmailSendingStatus } from './enter-email.service';

@Component({
	selector: 'ct-enter-email',
	templateUrl: 'enter-email.component.html'
})

export class EnterEmailComponent {
	emailSubmitted: boolean = false;
	errorCode: number;

	constructor() {
	}

	displayEmailSubmittedMessage(isEmailValid: EmailSendingStatus): void {
		this.errorCode = isEmailValid.message;
		this.emailSubmitted = isEmailValid.isSentEmail;
	}
}
