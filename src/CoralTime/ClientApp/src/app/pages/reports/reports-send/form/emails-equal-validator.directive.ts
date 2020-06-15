import { Directive, forwardRef } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';

@Directive({
	selector: '[ctEmailsEqualValidator]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => EmailsEqualValidatorDirective),
			multi: true
		}
	]
})

export class EmailsEqualValidatorDirective implements Validator {
	validate(c: AbstractControl): {[key: string]: any} {
		if (!c.value['emails']) {
			return {ctEmailInvalid: true};
		}

		let emails = c.value['emails'];

		for (let i = 0; i < emails.length; i++) {
			let equalInputValues = emails.filter((compareTask: string) => {
				return compareTask && emails[i] && emails[i].toLowerCase() === compareTask.toLowerCase();
			});
			if (equalInputValues.length > 1) {
				return {ctEmailsEqual: true};
			}
		}

		return null;
	}
}
