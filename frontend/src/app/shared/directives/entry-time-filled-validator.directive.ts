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
		const timeActual: number = this.convertTimeStringToNumber(v['timeActual']);
		const timeEstimated: number = this.convertTimeStringToNumber(v['timeEstimated']);

		if (timeActual === 0 && timeEstimated === 0) {
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
