import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { Client } from '../models/client';

@Injectable()
export class ClientsService {
	readonly odata: ODataService<Client>;

	constructor(private odataFactory: ODataServiceFactory) {
		this.odata = this.odataFactory.CreateService<Client>('Clients');
	}

	getClients(): Observable<Client[]> {
		let filters = [];
		let query = this.odata
			.Query();

		query.OrderBy('name asc');
		filters.push('isActive eq true');
		query.Filter(filters.join(' and '));

		return query.Exec().map(res => res.map((x: Object) => new Client(x)));
	}

	getClientsWithCount(event, filterStr = '', isActive: boolean = true): Observable<PagedResult<Client>> {
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
			filters.push('contains(tolower(name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('isActive eq ' + isActive);
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: Object) => new Client(x));
			return res;
		});
	}

	getManagerClientsWithCount(event, filterStr = '', isActive: boolean = true): Observable<PagedResult<Client>> {
		let odata = this.odataFactory.CreateService<Client>('ManagerClients');

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
			filters.push('contains(tolower(name),\'' + filterStr.trim().toLowerCase() + '\')');
		}

		filters.push('isActive eq ' + isActive);
		query.Filter(filters.join(' and '));

		return query.ExecWithCount().map(res => {
			res.data = res.data.map((x: Object) => new Client(x));
			return res;
		});
	}

	getClientByName(name: string): Observable<Client> {
		name = name.trim().toLowerCase();
		if (!name) {
			throw new Error('Please, specify client name');
		}

		let query = this.odata
			.Query()
			.Top(1);

		query.Filter('tolower(name) eq \'' + name + '\'');

		return query.Exec()
			.flatMap(result => {
				let client = result[0] ? new Client(result[0]) : null;
				return Observable.of(client);
			});
	}
}
