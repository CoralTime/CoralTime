import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgotPasswordComponent } from "./forgot-password.component";
import { SharedModule } from "../../shared/shared.module";
import { ForgotPasswordService } from './enter-new-password/forgot-password.service';
import { EnterNewPasswordComponent } from './enter-new-password/enter-new-password.component';
import { EnterEmailComponent } from './enter-email/enter-email.component';
import { ForgotPasswordRoutingModule } from './forgot-password-routing.module';
import { EnterEmailFormComponent } from './enter-email/enter-email-form/enter-email-form.component';
import { EnterNewPasswordFormComponent } from './enter-new-password/enter-new-password-form/enter-new-password-form.component';

@NgModule({
	imports: [
		CommonModule,
		SharedModule,
		ForgotPasswordRoutingModule
	],
	declarations: [
		ForgotPasswordComponent,
		EnterEmailFormComponent,
		EnterNewPasswordComponent,
		EnterEmailComponent,
		EnterNewPasswordFormComponent
	],
	providers: [
		ForgotPasswordService
	],
	exports: [
		ForgotPasswordComponent
	]
})

export class ForgotPasswordModule {
}