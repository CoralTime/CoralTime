import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SetPasswordComponent } from './set-password.component';
import { SharedModule } from '../../shared/shared.module';
import { SetPasswordService } from './enter-new-password/set-password.service';
import { EnterNewPasswordComponent } from './enter-new-password/enter-new-password.component';
import { EnterEmailComponent } from './enter-email/enter-email.component';
import { SetPasswordRoutingModule } from './set-password-routing.module';
import { EnterEmailFormComponent } from './enter-email/enter-email-form/enter-email-form.component';
import { EnterNewPasswordFormComponent } from './enter-new-password/enter-new-password-form/enter-new-password-form.component';

@NgModule({
	imports: [
		CommonModule,
		SharedModule,
		SetPasswordRoutingModule
	],
	declarations: [
		SetPasswordComponent,
		EnterEmailFormComponent,
		EnterNewPasswordComponent,
		EnterEmailComponent,
		EnterNewPasswordFormComponent
	],
	providers: [
		SetPasswordService
	],
	exports: [
		SetPasswordComponent
	]
})

export class SetPasswordModule {
}
