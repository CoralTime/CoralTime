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

//This is no longer being used by "entry-time-form.component.html" so it can probably be deleted at some point.
//The time actual "input" elements are not available when the IsFromToRequired setting is set to true so we can't use this anymore.
//The times are now validated inside "entry-time-form.component.cs"
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
