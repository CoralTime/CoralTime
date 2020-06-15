import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
	selector: '[notEmptyInputListValidator]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => NotEmptyInputListValidator),
			multi: true
		}
	]
})

export class NotEmptyInputListValidator implements Validator {
	constructor() {}

	validate(control: AbstractControl): {[key: string]: any} {
		let formValue: any = control.value['tasks'];
		let filledInputsNumber = 0;

		formValue.forEach((value: string) => {
			if (value) {
				filledInputsNumber++;
			}
		});

		if (filledInputsNumber === 0) {
			return {notEmptyInputList: false};
		}

		return null;
	}
}
