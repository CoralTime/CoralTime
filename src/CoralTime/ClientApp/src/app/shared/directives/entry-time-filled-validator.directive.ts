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
		const v = c.value;
		const timeActual: number = this.convertTimeStringToNumber(v['timeActualHours'], v['timeActualMinutes']);
		const timeEstimated: number = this.convertTimeStringToNumber(v['timeEstimatedHours'], v['timeEstimatedMinutes']);

		if (timeActual === 0 && timeEstimated === 0) {
			return {
				EntryTimeFilled: false
			};
		}

		return null;
	}

	private convertTimeStringToNumber(hours: string, minutes: string): number {
		return Number(hours || 0) * 60 + Number(minutes || 0)
	}
}
