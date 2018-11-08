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
		let invalidValue = '00';

		if ((
			!v['timeEstimatedHours'] &&
			invalidValue.includes(v['timeActualHours']) &&
			invalidValue.includes(v['timeActualMinutes'])
		) || (
			invalidValue.includes(v['timeActualHours']) &&
			invalidValue.includes(v['timeActualMinutes']) &&
			invalidValue.includes(v['timeEstimatedHours']) &&
			invalidValue.includes(v['timeEstimatedMinutes'])
		)) {
			return {
				EntryTimeFilled: false
			};
		}

		return null;
	}
}
