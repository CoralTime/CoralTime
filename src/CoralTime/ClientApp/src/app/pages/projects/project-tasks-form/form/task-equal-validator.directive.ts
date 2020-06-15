import { Directive, forwardRef, Input } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';
import { Task } from '../../../../models/task';

@Directive({
	selector: '[ctTaskEqualValidator]',
	providers: [
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => TaskEqualValidatorDirective),
			multi: true
		}
	]
})

export class TaskEqualValidatorDirective implements Validator {
	@Input('ctTaskEqualValidator') projectTasks: Task[] = [];

	validate(c: AbstractControl): { [key: string]: any } {
		let inputValue: string = c.value;

		if (!inputValue) {
			return {ctTaskInvalid: true};
		}

		if (this.projectTasks) {
			let assignedTask = this.projectTasks.filter((compareTask: Task) => {
				return compareTask && inputValue && inputValue.toLowerCase() === compareTask.name.toLowerCase();
			});
			if (assignedTask.length > 0) {
				return {taskAlreadyExist: true};
			}
		}

		return null;
	}
}
