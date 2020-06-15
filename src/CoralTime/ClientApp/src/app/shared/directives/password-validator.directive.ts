import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
	selector: '[ctPasswordValidator][formControlName],[ctPasswordValidator][formControl],[ctPasswordValidator][ngModel]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => PasswordValidator),
			multi: true
		}
	]
})

export class PasswordValidator implements Validator {
	validate(c: AbstractControl): {[key: string]: any} {
		if (!c.value || c.value.length < 8) {
			return {ctPasswordLengthInvalid: true};
		}

		if (!/[A-Z]/.test(c.value)) {
			return {ctPasswordUppercaseInvalid: true};
		}

		if (!/[a-z]/.test(c.value)) {
			return {ctPasswordLowercaseInvalid: true};
		}

		if (!/\d+/.test(c.value)) {
			return {ctPasswordDigitInvalid: true};
		}

		if (!/[{}!"#$%&'()*+,-./:;<=>?@\[\\\]^_`|~]/.test(c.value)) {
			return {ctPasswordSpecialCharacterInvalid: true};
		}

		return null;
	}
}
