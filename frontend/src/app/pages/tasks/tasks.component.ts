import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

import { PagedResult } from '../../services/odata/query';
import { TasksService } from '../../services/tasks.service';
import { NotificationService } from '../../core/notification.service';
import { Task } from '../../models/task';
import { TaskFormComponent } from './form/tasks-form.component';
import { Subject } from 'rxjs';
import { AclService } from '../../core/auth/acl.service';
import { ImpersonationService } from '../../services/impersonation.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';

@Component({
	selector: 'ct-tasks',
	templateUrl: 'tasks.component.html'
})

export class TasksComponent implements OnInit {
	isActiveTab: boolean = true;
	isAllTasks: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<Task>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private lastEvent: any;
	private subject = new Subject<any>();

	private dialogRef: MatDialogRef<TaskFormComponent>;

	constructor(private aclService: AclService,
	            private dialog: MatDialog,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private tasksService: TasksService) {
		this.impersonationService.checkImpersonationRole('tasks');
	}

	ngOnInit() {
		this.getTasks();
	}

	onEndScroll(): void {
		this.checkIsAllTasks();

		if (!this.isAllTasks) {
			this.loadLazy();
		}
	}

	getTasks(): void {
		this.subject.debounceTime(500).switchMap(() => {
			if (this.aclService.isGranted('roleAddProject')) {
				return this.tasksService.getTasksWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
			} else {
				return this.tasksService.getManagerTasksWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
			}
		})
			.subscribe(
				(res: PagedResult<Task>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}
					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
				},
				error => this.notificationService.danger('Error loading tasks.')
			);
	}

	loadLazy(event = null, updatePage?: boolean): void {
		this.checkIsAllTasks();

		if (event) {
			this.lastEvent = event;
			this.isAllTasks = false;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.lastEvent.first = 0;
			this.isAllTasks = false;
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllTasks) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllTasks(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllTasks = true;
		}
	}

	openTaskDialog(task: Task = null): void {
		this.dialogRef = this.dialog.open(TaskFormComponent);
		this.dialogRef.componentInstance.task = task;

		this.dialogRef.componentInstance.onSubmit.subscribe((response) => {
			this.dialogRef.close();
			this.onSubmit(response);
		});
	}

	onSubmit(response: any): void {
		if (response.error) {
			this.notificationService.danger('Error saving task.');
			return;
		}

		if (response.isNewTask) {
			this.notificationService.success('New task has been successfully created.');
		} else {
			this.notificationService.success('Task has been successfully changed.');
		}

		this.loadLazy(null, true);
	}

	toggleTab(isActiveTab: boolean): void {
		if (this.lastEvent) {
			this.lastEvent.first = 0;
			this.lastEvent.pageCount = 1;
		}
		this.isActiveTab = isActiveTab;
		this.loadLazy(null, true);
	}

	onResize(): void {
		this.resizeObservable.next();
	}
}
