import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PasswordChangingStatus, SetPasswordService } from './set-password.service';

@Component({
	selector: 'ct-enter-new-password',
	templateUrl: 'enter-new-password.component.html'
})

export class EnterNewPasswordComponent implements OnInit {
	isPasswordChanged: boolean = false;

	constructor(private route: ActivatedRoute,
	            private router: Router,
	            private setPasswordService: SetPasswordService) {
	}

	ngOnInit(): void {
		this.route.data.forEach((data: { restoreCodeValid: boolean }) => {
			if (!data.restoreCodeValid) {
				this.setPasswordService.restoreCodeIsExpired = !data.restoreCodeValid;
				this.router.navigate(['set-password']);
			}
		});
	}

	displayNewPasswordChangedMessage(paswordChanged: PasswordChangingStatus): void {
		this.isPasswordChanged = paswordChanged.isChangedPassword;
	}
}
