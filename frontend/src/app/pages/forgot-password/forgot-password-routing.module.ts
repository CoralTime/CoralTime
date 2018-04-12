import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ForgotPasswordComponent } from './forgot-password.component';
import { EnterEmailComponent } from './enter-email/enter-email.component';
import { EnterNewPasswordComponent } from './enter-new-password/enter-new-password.component';
import { EnterEmailService } from './enter-email/enter-email.service';
import { ForgotPasswordService } from './enter-new-password/forgot-password.service';
import { ValidateRestoreCodeResolve } from './enter-new-password/validate-activation-code-resolve.service';

@NgModule({
	imports: [
		RouterModule.forChild([
			{
				path: '',
				component: ForgotPasswordComponent,
				children: [
					{
						path: '',
						component: EnterEmailComponent
					},
					{
						path: 'enter-new-password',
						component: EnterNewPasswordComponent,
						resolve: {
							restoreCodeValid: ValidateRestoreCodeResolve
						}
					}
				]
			}
		])
	],
	providers: [
		EnterEmailService,
		ForgotPasswordService,
		ValidateRestoreCodeResolve
	],
	exports: [
		RouterModule
	]
})

export class ForgotPasswordRoutingModule {
}
