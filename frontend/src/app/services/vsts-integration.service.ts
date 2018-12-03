import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { VstsProjectConnection, VstsUser } from '../models/vsts-project-connection';
import { ConstantService } from '../core/constant.service';

@Injectable()
export class VstsIntegrationService {
	readonly odata: ODataService<VstsProjectConnection>;

	constructor(private constantService: ConstantService,
	            private http: HttpClient,
	            private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<VstsProjectConnection>('VstsProjectIntegration');
	}

	getConnectionsWithCount(event, filterStr = ''): Observable<PagedResult<VstsProjectConnection>> {
		const filters = [];
		const query = this.odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('projectName' + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		}

		if (filterStr) {
			filters.push('contains(tolower(projectName),\'' +
				filterStr.trim().toLowerCase() + '\')' +
				' or contains(tolower(vstsProjectName),\'' +
				filterStr.trim().toLowerCase() + '\')');
		}

		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: Object) => new VstsProjectConnection(x));
			return res;
		});
	}

	getConnectionsByProjectName(name: string): Observable<VstsProjectConnection> {
		name = name.trim().toLowerCase();
		if (!name) {
			throw new Error('Please, specify project name');
		}

		const query = this.odata
			.Query()
			.Top(1);

		query.Filter('tolower(vstsProjectName) eq \'' + name + '\'');

		return query.Exec()
			.flatMap(result => {
				let project = result[0] ? new VstsProjectConnection(result[0]) : null;
				return Observable.of(project);
			});
	}

	getConnectionsMembersWithCount(event, filterStr = '', connectionId: number): Observable<PagedResult<any>> {
		const odata = this.odataFactory.CreateService<VstsUser>('VstsProjectIntegration(' + connectionId + ')/members');

		const filters = [];
		const query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('fullName' + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		}

		if (filterStr) {
			filters.push('contains(tolower(fullName),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: Object) => new VstsUser(x));
			return res;
		});
	}

	updateVstsUsers(): Observable<any> {
		return this.http.get(this.constantService.adminApi + 'UpdateVstsUsers');
	};
}
