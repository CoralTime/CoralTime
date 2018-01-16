import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { Task } from '../models/task';

@Injectable()
export class TasksService {
	readonly odata: ODataService<Task>;

	constructor(private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<Task>('Tasks');
	}

	getTasks(): Observable<Task[]> {
		return this.odata.Query().Exec().map(res => res.map((x: any) => {
			return new Task(x);
		}));
	}

	getTasksWithCount(event, filterStr = '', isActive: boolean = true): Observable<PagedResult<Task>> {
		let filters = [];
		let query = this.odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('name' + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		}

		if (filterStr) {
			filters.push('contains(tolower(Name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('IsActive eq ' + isActive);
		filters.push('ProjectId eq null');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new Task(x));
			return res;
		});
	}

	getManagerTasksWithCount(event, filterStr = '', isActive: boolean = true): Observable<PagedResult<Task>> {
		let odata = this.odataFactory.CreateService<Task>('ManagerTasks');

		let filters = [];
		let query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('name' + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		}

		if (filterStr) {
			filters.push('contains(tolower(Name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('IsActive eq ' + isActive);
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new Task(x));
			return res;
		});
	}

	toggleActive(task: Task): Observable<any> {
		task.isActive = !task.isActive;

		return this.odata.Patch({
			isActive: task.isActive
		}, task.id.toString());
	}

	getProjectTasks(event, filterStr = '', projectId: number): Observable<PagedResult<Task>> {
		let filters = [];
		let query = this.odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('name asc');
		}

		if (filterStr) {
			filters.push('contains(tolower(Name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('IsActive eq true');
		filters.push('(ProjectId eq ' + projectId + ' or ProjectId eq null)');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new Task(x));
			return res;
		});
	}

	getTaskByName(name: string): Observable<Task> {
		name = name.trim().toLowerCase();
		if (!name) {
			throw new Error('Please, specify task name');
		}

		let query = this.odata
			.Query()
			.Top(1);

		query.Filter('tolower(Name) eq \'' + name + '\' and ProjectId eq null');

		return query.Exec()
			.flatMap(result => {
				let task = result[0] ? new Task(result[0]) : null;
				return Observable.of(task);
			});
	}

	getActiveTasks(projectId?: number): Observable<PagedResult<Task>> {
		let filters = [];
		let query = this.odata
			.Query();

		query.OrderBy('name asc');
		if (projectId) {
			filters.push('(ProjectId eq ' + projectId + ' or ProjectId eq null)');
		} else {
			filters.push('ProjectId eq null');
		}
		filters.push('IsActive eq true');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new Task(x));
			return res;
		});
	}
}
