import { Directive, forwardRef } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';

const EMAIL_REGEXP = /^[a-zA-Z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$/;

@Directive({
	selector: '[ctEmailsInvalidValidator]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => EmailInvalidValidatorDirective),
			multi: true
		}
	]
})

export class EmailInvalidValidatorDirective implements Validator {
	validate(c: AbstractControl): { [key: string]: any } {
		if (!c.value['emails']) {
			return {ctEmailInvalid: true};
		}

		let emails = c.value['emails'];

		for (let i = 0; i < emails.length; i++) {
			if (!emails[i]) {
				return {ctEmailEmpty: true};
			}
			if (!EMAIL_REGEXP.test(emails[i])) {
				return {ctEmailInvalid: true};
			}
		}

		return null;
	}
}
