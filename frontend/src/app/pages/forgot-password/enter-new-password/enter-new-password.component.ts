import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PasswordChangingStatus, ForgotPasswordService } from './forgot-password.service';

@Component({
	selector: 'ct-enter-new-password',
	templateUrl: 'enter-new-password.component.html'
})

export class EnterNewPasswordComponent implements OnInit {
	isPasswordChanged: boolean = false;

	constructor(private forgotPasswordService: ForgotPasswordService,
	            private route: ActivatedRoute,
	            private router: Router) {
	}

	ngOnInit(): void {
		this.route.data.forEach((data: { restoreCodeValid: boolean }) => {
			if (!data.restoreCodeValid) {
				this.forgotPasswordService.restoreCodeIsExpired = !data.restoreCodeValid;
				this.router.navigate(['forgot-password']);
			}
		});
	}

	displayNewPasswordChangedMessage(paswordChanged: PasswordChangingStatus): void {
		this.isPasswordChanged = paswordChanged.isChangedPassword;
	}
}