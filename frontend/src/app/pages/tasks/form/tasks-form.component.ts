import { Observable } from 'rxjs/Observable';
import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { Task } from '../../../models/task';
import { TasksService } from '../../../services/tasks.service';
import { TranslatePipe } from '@ngx-translate/core';
import { ArrayUtils } from '../../../core/object-utils';

class FormTask {
	id: number;
	name: string;
	description: string;
	isActive: boolean;

	static formTask(task: Task) {
		let instance = new this;
		instance.id = task.id;
		instance.name = task.name;
		instance.description = task.description;
		instance.isActive = task.id ? task.isActive : true;

		return instance;
	}

	toTask(task: Task) {
		task.id = this.id;
		task.name = this.name;
		task.description = this.description;
		task.isActive = this.isActive;

		return task;
	}
}

@Component({
	selector: 'ct-task-form',
	templateUrl: 'tasks-form.component.html',
	providers: [TranslatePipe]
})

export class TaskFormComponent implements OnInit {
	dialogHeader: string;
	submitButtonText: string;
	task: Task;
	errorMessage: string;

	isActive: boolean;
	stateModel: any;
	stateText: string;
	isRequestLoading: boolean = false;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'archived'}
	];

	model: FormTask;

	@Output() onSubmit = new EventEmitter();

	private isNewTask: boolean;

	constructor(private tasksService: TasksService, private translatePipe: TranslatePipe) {}

	ngOnInit() {
		let task = this.task;
		this.isNewTask = !task;
		this.task = task ? task : new Task();
		this.submitButtonText = this.task.id ? 'Save' : 'Create';
		this.dialogHeader = this.task.id ? 'Edit' : this.translatePipe.transform('Create New Task');
		this.model = FormTask.formTask(this.task);
		this.stateModel = ArrayUtils.findByProperty(this.states, 'value', this.model.isActive);

		this.stateText = this.task.isActive ? '' : 'Archived task is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';

	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'Archived task is not suggested for time tracking in calendar. Time entries are read only for team members, but still editable for managers.';
	}

	submit(): void {
		this.errorMessage = null;
		this.task = this.model.toTask(this.task);

		if (!this.task.name) {
			this.errorMessage = 'Task name is required.';
		} else {
			let submitObservable: Observable<any>;

			if (this.task.id) {
				submitObservable = this.tasksService.odata.Put(this.task, this.task.id.toString());
			} else {
				submitObservable = this.tasksService.odata.Post(this.task);
			}

			this.isRequestLoading = true;
			submitObservable.toPromise().then(() => {
				this.isRequestLoading = false;
				this.onSubmit.emit(this.isNewTask);
			});
		}
	}
}