import { Task } from '../../models/task';
import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { TasksService } from '../../services/tasks.service';

@Directive({
	selector: '[ctTaskNameValidator][formControlName],[ctTaskNameValidator][formControl],[ctTaskNameValidator][ngModel]',
	providers: [
		{
			provide: NG_ASYNC_VALIDATORS,
			useExisting: forwardRef(() => TaskNameValidator),
			multi: true
		}
	]
})

export class TaskNameValidator implements Validator {
	@Input('ctTaskNameValidator') task: Task;

	constructor(private tasksService: TasksService) {
	}

	validate(control: AbstractControl): Observable<{ [key: string]: any }> {
		return control.valueChanges
			.debounceTime(500)
			.take(1)
			.switchMap(() => {
				return this.tasksService.getTaskByName(control.value);
			})
			.map(task => {
				if (task && (!this.task || task.id !== this.task.id)) {
					return {ctTaskNameInvalid: true};
				}

				return null;
			})
			.first()
	}
}
