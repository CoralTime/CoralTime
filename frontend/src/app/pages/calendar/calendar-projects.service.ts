import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { ODataService } from '../../services/odata/odata';
import { Project } from '../../models/project';
import { ODataServiceFactory } from '../../services/odata/odataservicefactory';

@Injectable()
export class CalendarProjectsService {
	readonly odata: ODataService<Project>;

	projects: Project[] = [];
	filteredProjects: number[] = [];

	constructor(private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<Project>('Projects');
	}

	clearProject(): void {
		this.projects = [];
	}

	getProjects(showOnlyActive): Observable<Project[]> {
		if(this.projects.length) {
			return Observable.of(this.projects);
		}
		return this.loadProjects(showOnlyActive);
	}

	loadProjects(showOnlyActive: boolean): Observable<Project[]> {
		let filters = [];
		let query = this.odata
			.Query();

		query.OrderBy('name asc');

		if (showOnlyActive) {
			filters.push('IsActive eq true');
		}

		query.Filter(filters.join(' and '));

		return query.Exec().map(res => {
			this.projects = res.map((x: any) => new Project(x));
			return this.projects;
		});
	}
}
