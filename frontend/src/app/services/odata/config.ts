import { HttpResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PagedResult } from './query';

export class KeyConfigs {
	expand: string = '$expand';
	filter: string = '$filter';
	orderBy: string = '$orderby';
	select: string = '$select';
	skip: string = '$skip';
	top: string = '$top';
}

@Injectable()
export class ODataConfiguration {
	baseUrl: string = 'http://localhost/odata';
	keys: KeyConfigs = new KeyConfigs();

	getEntityUri(entityKey: string, _typeName: string): string {
		// check if string is a GUID (UUID) type
		if (/^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(entityKey)) {
			return this.baseUrl + '/' + _typeName + '(' + entityKey + ')';
		}

		if (!/^[0-9]*$/.test(entityKey)) {
			return this.baseUrl + '/' + _typeName + '(\'' + entityKey + '\')';
		}

		return this.baseUrl + '/' + _typeName + '(' + entityKey + ')';
	}

	handleError(err: any, caught: any): void {
		console.warn('OData error: ', err, caught);
	};

	get requestOptions() {
		return {headers: new HttpHeaders()};
	};

	get postRequestOptions() {
		let headers = new HttpHeaders()
			.append('Content-Type', 'application/json; charset=utf-8');

		return {headers: headers, observe: 'response' as 'response'};
	}

	extractQueryResultData<T>(res: HttpResponse<T>): T[] {
		if (res.status < 200 || res.status >= 300) {
			throw new Error('Bad response status: ' + res.status);
		}

		let entities: T[] = res['value'];
		return entities;
	}

	extractQueryResultDataWithCount<T>(res: HttpResponse<T>): PagedResult<T> {
		let pagedResult = new PagedResult<T>();

		if (res.status < 200 || res.status >= 300) {
			throw new Error('Bad response status: ' + res.status);
		}
		let entities: T[] = res['value'];

		pagedResult.data = entities;

		try {
			let count = parseInt(res['@odata.count'], 10) || entities.length;
			pagedResult.count = count;
		} catch (error) {
			console.warn('Cannot determine OData entities count. Falling back to collection length...');
			pagedResult.count = entities.length;
		}

		return pagedResult;
	}
}
