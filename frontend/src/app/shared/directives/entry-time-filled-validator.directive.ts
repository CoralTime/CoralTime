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
		let invalidValue = '00:00';

		if (invalidValue.indexOf(v['actualTime']) + 1 &&
			invalidValue.indexOf(v['plannedTime']) + 1) {
			return {
				EntryTimeFilled: false
			};
		}

		return null;
	}
}
