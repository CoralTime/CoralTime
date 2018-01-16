import { Component, Output, EventEmitter, Input } from '@angular/core';
import { Project } from '../../../../models/project';
import { TasksService } from '../../../../services/tasks.service';
import { Task } from '../../../../models/task';
import { FormArray, FormBuilder, FormControl, FormControlName, FormGroup, Validators } from '@angular/forms';
import { NotificationService } from '../../../../core/notification.service';

@Component({
	selector: 'ct-project-tasks-form',
	templateUrl: 'project-tasks-form.component.html'
})

export class ProjectTasksFormComponent {
	@Input() project: Project;
	@Input() projectTasks: Task[];

	@Output() onTaskSubmitted = new EventEmitter();
	@Output() onHeightChanged: EventEmitter<any> = new EventEmitter();

	form: FormGroup;
	formHeight: number;
	isRequestLoading: boolean;

	constructor(private fb: FormBuilder,
	            private notificationService: NotificationService,
	            private tasksService: TasksService) {
		this.form = this.fb.group({
			tasks: new FormArray([])
		});

		const originFormControlNameNgOnChanges = FormControlName.prototype.ngOnChanges;
		FormControlName.prototype.ngOnChanges = function() {
			const result =  originFormControlNameNgOnChanges.apply(this, arguments);
			this.control.nativeElement = this.valueAccessor._elementRef.nativeElement;
			return result;
		};

		this.onFormHeightChanged();
	}

	addNewTask(): void {
		if (this.tasks.length > 4) {
			return;
		}

		const arrayControl = <FormArray>this.form.controls['tasks'];
		let newControl = new FormControl('', Validators.required);
		arrayControl.push(newControl);
		this.onFormHeightChanged();

		setTimeout(() => (<any>this.tasks.get(this.tasks.length - 1 + '')).nativeElement.focus(), 0);
	}

	delTask(index: number): void {
		const arrayControl = <FormArray>this.form.controls['tasks'];
		arrayControl.removeAt(index);
		this.onFormHeightChanged();
	}

	submitTask(index: number, control: FormControl): void {
		if (control.hasError('ctTaskInvalid')) {
			this.notificationService.danger('Task can\'t be empty.');
			return;
		}

		if (this.isTaskAlreadyExist(control.value)) {
			this.notificationService.danger('Task with the same name already exists.');
			return;
		}

		let projectTask: Task = new Task({
			projectId: this.project.id,
			name: control.value,
			isActive: true
		});

		this.isRequestLoading = true;
		this.tasksService.odata.Post(projectTask)
			.subscribe(() => {
					this.isRequestLoading = false;
					this.delTask(index);
					this.onTaskSubmitted.emit();
				},
				(error) => {
					this.isRequestLoading = false;
					this.onTaskSubmitted.emit(error);
				});
	}

	get tasks(): FormArray { return this.form.get('tasks') as FormArray; }

	private isTaskAlreadyExist(inputValue: string): boolean {
		let assignedTask = this.projectTasks.filter((compareTask: Task) => {
			return compareTask && inputValue && inputValue.toLowerCase() === compareTask.name.toLowerCase();
		});

		if (assignedTask.length > 0) {
			return true;
		}

		return false;
	}

	private onFormHeightChanged(): void {
		this.onHeightChanged.emit(this.tasks.length);
	}
}
