import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Subject } from 'rxjs';
import { Task } from '../../models/task';
import { PagedResult } from '../../services/odata';
import { AclService } from '../../core/auth/acl.service';
import { ROWS_ON_PAGE } from '../../core/constant.service';
import { NotificationService } from '../../core/notification.service';
import { ImpersonationService } from '../../services/impersonation.service';
import { TasksService } from '../../services/tasks.service';
import { TaskFormComponent } from './form/tasks-form.component';

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
	}

	ngOnInit() {
		this.getTasks();
	}

	// GRID DISPLAYING

	loadLazy(event = null, updatePage?: boolean): void {
		if (event) {
			this.lastEvent = event;
		}
		if (updatePage) {
			this.updatingGrid = true;
			this.lastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllTasks = false;
			this.pagedResult = null;
			this.resizeObservable.next(true);
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

	onEndScroll(): void {
		if (!this.isAllTasks) {
			this.loadLazy();
		}
	}

	private checkIsAllTasks(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllTasks = true;
		}
	}

	private getTasks(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.tasksService.getTasksWithCount(this.lastEvent, this.filterStr, this.isActiveTab);
		})
			.subscribe((res: PagedResult<Task>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}

					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
					this.checkIsAllTasks();
				},
				() => this.notificationService.danger('Error loading tasks.')
			);
	}

	// FORM

	openTaskDialog(task: Task = null): void {
		if (!this.aclService.isGranted('roleEditTask')) {
			return;
		}

		this.dialogRef = this.dialog.open(TaskFormComponent);
		this.dialogRef.componentInstance.task = task;

		this.dialogRef.componentInstance.onSubmit.subscribe((response) => {
			this.dialogRef.close();
			this.onSubmit(response);
		});
	}

	private onSubmit(response: any): void {
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

	// GENERAL

	onResize(): void {
		this.resizeObservable.next();
	}

	toggleTab(isActiveTab: boolean): void {
		if (this.lastEvent) {
			this.lastEvent.first = 0;
			this.lastEvent.pageCount = 1;
		}

		this.isActiveTab = isActiveTab;
		this.loadLazy(null, true);
	}
}
