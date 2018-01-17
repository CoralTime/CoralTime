import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Project } from '../../../models/project';
import { NotificationService } from '../../../core/notification.service';
import { Observable } from 'rxjs';
import { TasksService } from '../../../services/tasks.service';
import { Task } from '../../../models/task';
import { PagedResult } from '../../../services/odata/query';
import { Subject } from 'rxjs/Subject';
import { ROWS_ON_PAGE } from '../../../core/constant.service';

@Component({
	selector: 'ct-project-tasks',
	templateUrl: 'project-tasks.component.html'
})

export class ProjectTasksComponent implements OnInit {
	@Input() project: Project;

	@ViewChild('grid') gridContainer: ElementRef;

	filterStr: string = '';
	isActive: boolean;
	isAllTasks: boolean = false;
	resizeObservable: Subject<any> = new Subject();
	tasksPagedResult: PagedResult<Task>;
	updatingGrid: boolean = false;

	private tasksSubject = new Subject<any>();
	private tasksLastEvent: any;

	constructor(private tasksService: TasksService,
	            private notificationService: NotificationService) {
	}

	ngOnInit() {
		this.loadTasks();
	}

	loadTasks(): void {
		this.tasksSubject.debounceTime(500).switchMap(() => {
			return this.tasksService.getProjectTasks(this.tasksLastEvent, this.filterStr, this.project.id);
		})
			.subscribe(
				(res: PagedResult<Task>) => {
					if (!this.tasksPagedResult || !this.tasksLastEvent.first || this.updatingGrid) {
						this.tasksPagedResult = res;
					} else {
						this.tasksPagedResult.data = this.tasksPagedResult.data.concat(res.data);
					}

					if (!this.filterStr) {
						this.project.tasksCount = this.tasksPagedResult.count;
					}

					this.tasksLastEvent.first = this.tasksPagedResult.data.length;
					this.updatingGrid = false;
				},
				error => this.notificationService.danger('Error loading tasks.')
			);
	}

	onFormHeightChanged(tasksNumber: number): void {
		this.changeScrollableContainer(tasksNumber);
		this.resizeObservable.next();
	}

	onTaskSubmitted(error): void {
		if (error) {
			this.notificationService.danger('Error creating adding task.');
		} else {
			this.filterStr = '';
			this.updateTasks(null, true);
			this.notificationService.success('Task successfully assigned.');
		}
	}

	removeTask(task: Task): void {
		let observable: Observable<any>;

		observable = this.tasksService.toggleActive(task);
		observable.subscribe(
			() => {
				this.filterStr = '';
				this.updateTasks(null, true);
				this.notificationService.success('Task status has been successfully removed from projects.');
			}
		);
	}

	onEndScroll(): void {
		this.checkIsAllTasks();

		if (!this.isAllTasks) {
			this.updateTasks();
		}
	}

	updateTasks(event = null, updatePage?: boolean): void {
		this.checkIsAllTasks();

		if (event) {
			this.tasksLastEvent = event;
			this.isAllTasks = false;
		}
		if (updatePage) {
			this.updatingGrid = updatePage;
			this.tasksLastEvent.first = 0;
			this.isAllTasks = false;
		}
		this.tasksLastEvent.rows = ROWS_ON_PAGE;

		this.tasksSubject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllTasks(): void {
		if (this.tasksPagedResult && this.tasksPagedResult.data.length >= this.tasksPagedResult.count) {
			this.isAllTasks = true;
		}
	}

	onResize(): void {
		this.resizeObservable.next();
	}

	private changeScrollableContainer(tasksNumber: number): void {
		let grid = this.gridContainer.nativeElement;
		let wrappers = grid.querySelectorAll('.ui-datatable-scrollable-body');

		wrappers[0].setAttribute('style', 'max-height: calc(90vh - 176px - ' + tasksNumber * 40 + 'px)');
	}
}
