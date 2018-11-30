import { Directive, forwardRef } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
	selector: 'form[ctEntryTimeFilled]',
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

	private convertTimeStringToNumber(time: string): number {
		if (!time) {
			return 0;
		}

		const timeArray = time.split(':');
		return Number(timeArray[0]) * 60 + Number(timeArray[1] || 0)
	}
}
