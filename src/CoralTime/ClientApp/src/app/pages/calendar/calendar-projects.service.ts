import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { ODataService } from '../../services/odata/odata';
import { Project } from '../../models/project';
import { ODataServiceFactory } from '../../services/odata/odataservicefactory';
import { ImpersonationService } from '../../services/impersonation.service';
import { AuthService } from '../../core/auth/auth.service';

@Injectable()
export class CalendarProjectsService {
	readonly odata: ODataService<Project>;

	defaultProject: Project;
	filteredProjects: number[] = [];
	projects: Project[] = [];

	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<Project>('Projects');

		if (localStorage.hasOwnProperty('DEFAULT_PROJECT')) {
			this.defaultProject = JSON.parse(localStorage.getItem('DEFAULT_PROJECT'));
		}

		this.impersonationService.onChange.subscribe(() => {
			this.projects = [];
			this.setDefaultProject(null);
		});

		this.authService.onChange.subscribe(() => {
			this.setDefaultProject(null);
		});
	}

	clearProject(): void {
		this.projects = [];
	}

	getProjects(showOnlyActive): Observable<Project[]> {
		if (this.projects.length) {
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
			filters.push('isActive eq true');
		}

		query.Filter(filters.join(' and '));

		return query.Exec().map(res => {
			this.projects = res.map((x: any) => new Project(x));
			return this.projects;
		});
	}

	setDefaultProject(project: Project): void {
		this.defaultProject = project;

		if (project) {
			localStorage.setItem('DEFAULT_PROJECT', JSON.stringify(project));
		} else {
			localStorage.removeItem('DEFAULT_PROJECT');
		}
	}
}
