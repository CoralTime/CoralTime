import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SetPasswordComponent } from './set-password.component';
import { EnterEmailComponent } from './enter-email/enter-email.component';
import { EnterNewPasswordComponent } from './enter-new-password/enter-new-password.component';
import { EnterEmailService } from './enter-email/enter-email.service';
import { SetPasswordService } from './enter-new-password/set-password.service';
import { ValidateRestoreCodeResolve } from './enter-new-password/validate-activation-code-resolve.service';

@NgModule({
	imports: [
		RouterModule.forChild([
			{
				path: '',
				component: SetPasswordComponent,
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
		SetPasswordService,
		ValidateRestoreCodeResolve
	],
	exports: [
		RouterModule
	]
})

export class SetPasswordRoutingModule {
}
