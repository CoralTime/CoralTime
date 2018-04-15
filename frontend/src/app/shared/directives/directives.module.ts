import { NgModule } from '@angular/core';
import { IsGrantedDirective } from './is-granted.directive';
import { EqualValidator } from './equal-validator.directive';
import { UsernameValidator } from './username-validator.directive';
import { ProjectNameValidator } from './project-name-validator.directive';
import { NumberOnlyDirective } from './number-only.directive';
import { TimeDirective } from './time.directive';
import { NotEmptyInputListValidator } from '../form/not-empy-input-list-validator.directive';
import { PasswordValidator } from './password-validator.directive';
import { ClickOutsideDirective } from './click-outside.directive';
import { TaskNameValidator } from './task-name.validator.directive';
import { ClientNameValidator } from './client-name-validator.directive';
import { EntryTimeFilledValidator } from './entry-time-filled-validator.directive';
import { ValidationOnBlurDirective } from './validation-onblur.directive';
import { EmailValidator } from './email-validator.directive';
import { FocusDirective } from './autofocus.directive';
import { DisableWhenRequestDirective } from './disable-when-request.directive';
import { NgForIn } from './ngForIn.directive';
import { ClickCloseDirective } from './click-close.directive';

@NgModule({
	imports: [],
	declarations: [
		IsGrantedDirective,
		EqualValidator,
		UsernameValidator,
		EmailValidator,
		ProjectNameValidator,
		NumberOnlyDirective,
		TimeDirective,
		ProjectNameValidator,
		NotEmptyInputListValidator,
		PasswordValidator,
		ClickOutsideDirective,
		FocusDirective,
		TaskNameValidator,
		ClientNameValidator,
		EntryTimeFilledValidator,
		ValidationOnBlurDirective,
		DisableWhenRequestDirective,
		NgForIn,
		ClickCloseDirective
	],
	exports: [
		IsGrantedDirective,
		EqualValidator,
		UsernameValidator,
		EmailValidator,
		FocusDirective,
		NumberOnlyDirective,
		TimeDirective,
		ProjectNameValidator,
		NotEmptyInputListValidator,
		PasswordValidator,
		ClickOutsideDirective,
		TaskNameValidator,
		ClientNameValidator,
		EntryTimeFilledValidator,
		ValidationOnBlurDirective,
		DisableWhenRequestDirective,
		NgForIn,
		ClickCloseDirective
	]
})

export class DirectivesModule {
}
