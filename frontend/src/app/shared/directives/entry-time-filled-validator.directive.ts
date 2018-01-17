import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
	selector: 'form[entryTimeFilled]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => EntryTimeFilledValidator),
			multi: true
		}
	]
})

export class EntryTimeFilledValidator implements Validator {
	validate(c: AbstractControl): { [key: string]: any } {
		let v = c.value;
		let invalidValues = ['', '0', '00'];

		if (invalidValues.indexOf(v['actualHours']) + 1 &&
			invalidValues.indexOf(v['actualMinutes']) + 1 &&
			invalidValues.indexOf(v['plannedHours']) + 1 &&
			invalidValues.indexOf(v['plannedMinutes']) + 1) {
			return {
				EntryTimeFilled: false
			};
		}

		return null;
	}
}
