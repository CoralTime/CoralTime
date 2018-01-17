import { UserProject } from '../models/user-project';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { User } from '../models/user';
import { Project } from '../models/project';

@Injectable()
export class UsersService {
	readonly odata: ODataService<User>;

	constructor(private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<User>('Members');
	}

	getUsersWithCount(event, filterStr = '', isActive?: boolean): Observable<PagedResult<User>> {
		let filters = [];
		let query = this.odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('fullName' + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		}

		if (filterStr) {
			filters.push('contains(tolower(UserName),\'' +
				filterStr.trim().toLowerCase() + '\')' +
				' or contains(tolower(FullName),\'' +
				filterStr.trim().toLowerCase() + '\')' +
				' or contains(tolower(Email),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		if (typeof isActive !== 'undefined') {
			filters.push('IsActive eq ' + isActive);
		}

		if (filters.length) {
			query.Filter(filters.join(' and '));
		}

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new User(x));
			return res;
		});
	}

	getProjectUsersWithCount(event, filterStr = '', projectId: number): Observable<PagedResult<UserProject>> {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');

		let filters = [];
		let query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy('memberName ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('memberName asc');
		}

		if (filterStr) {
			filters.push('contains(tolower(MemberName),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('ProjectId eq ' + projectId);
		filters.push('IsMemberActive eq true');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new UserProject(x));
			return res;
		});
	}

	getUnassignedUsersWithCount(event, filterStr = '', projectId: number): Observable<PagedResult<User>> {
		let odata = this.odataFactory.CreateService<User>('MemberProjectRoles(' + projectId + ')/Members');

		let filters = [];
		let query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy('FullName ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('FullName asc');
		}

		if (filterStr) {
			filters.push('contains(tolower(FullName),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('IsActive eq true');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new User(x));
			return res;
		});
	}

	getUserByUsername(username: string): Observable<User> {
		username = username.trim().toLowerCase();
		if (!username) {
			throw new Error('Please, specify username');
		}

		let query = this.odata
			.Query()
			.Top(1);

		query.Filter('tolower(UserName) eq \'' + username + '\'');

		return query.Exec()
			.flatMap(result => {
				let user = result[0] ? new User(result[0]) : null;
				return Observable.of(user);
			});
	}

	getUserByEmail(email: string): Observable<User> {
		email = email.trim().toLowerCase();
		if (!email) {
			throw new Error('Please, specify email');
		}

		let query = this.odata
			.Query()
			.Top(1);

		query.Filter('tolower(Email) eq \'' + email + '\'');

		return query.Exec()
			.flatMap(result => {
				let user = result[0] ? new User(result[0]) : null;
				return Observable.of(user);
			});
	}

	getUserById(id: number): Observable<User> {
		let query = this.odata
			.Query()
			.Top(1);

		query.Filter('Id eq ' + id);

		return query.Exec()
			.flatMap(result => {
				let user = result[0] ? new User(result[0]) : null;
				return Observable.of(user);
			});
	}

	getUserProjectsWithCount(event, filterStr = '', memberId: number): Observable<PagedResult<UserProject>> {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');

		let filters = [];
		let query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('ProjectName asc');
		}

		if (filterStr) {
			filters.push('contains(tolower(ProjectName),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('MemberId eq ' + memberId);
		filters.push('IsProjectActive eq true');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map(x => new UserProject(x));
			return res;
		});
	}

	getUnassignedProjectsWithCount(event, filterStr = '', memberId: number): Observable<PagedResult<Project>> {
		let odata = this.odataFactory.CreateService<Project>('MemberProjectRoles(' + memberId + ')/Projects');

		let filters = [];
		let query = odata
			.Query()
			.Top(event.rows)
			.Skip(event.first);

		if (event.sortField) {
			query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'asc' : 'desc'));
		} else {
			query.OrderBy('Name asc');
		}

		if (filterStr) {
			filters.push('contains(tolower(Name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('IsActive eq true');
		filters.push('IsPrivate eq true');
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: any) => new Project(x));
			return res;
		});
	}

	removeFromProject(userProject: UserProject): Observable<any> {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');

		return odata.Delete(userProject.id.toString());
	}

	assignProjectToUser(userId: number, projectId: number, roleId: number): Observable<any> {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');
		let userProject = new UserProject({
			projectId: projectId,
			roleId: roleId,
			memberId: userId
		});
		return odata.Post(userProject);
	}

	changeRole(userProjectId: number, roleId: number): Observable<any> {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');
		let newRoleId = {
			roleId: roleId
		};
		return odata.Patch(newRoleId, userProjectId.toString());
	}

	assignUserToProject(projectId: number, userId: number, roleId: number) {
		let odata = this.odataFactory.CreateService<UserProject>('MemberProjectRoles');
		let userProject = new UserProject({
			projectId: projectId,
			roleId: roleId,
			memberId: userId
		});
		return odata.Post(userProject);
	}
}
