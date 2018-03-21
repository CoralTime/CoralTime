import { Task } from '../../models/task';
import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/debounceTime';
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
	observable: Observable<any>;
	resolve: any = null;
	subject: Subject<string>;

	@Input('ctTaskNameValidator') private task: Task;

	constructor(private tasksService: TasksService) {
		this.subject = new Subject();
		this.observable = this.subject
			.debounceTime(500)
			.switchMap((taskName: string) => {
				return this.tasksService.getTaskByName(taskName);
			}).flatMap((task: Task) => {
				if (task && (!this.task || task.id !== this.task.id)) {
					return Observable.of({ctTaskNameInvalid: true});
				} else {
					return Observable.of(null);
				}
			});

		this.observable.subscribe((res) => {
			this.resolvePromise(res);
		});
	}

	resolvePromise(result): void {
		if (this.resolve) {
			this.resolve(result);
			this.resolve = null;
		}
	}

	validate(c: AbstractControl): Promise<{ [key: string]: any }> {
		this.resolvePromise(null);

		return new Promise(resolve => {
			this.subject.next(c.value);
			this.resolve = resolve;
		});
	}
}
