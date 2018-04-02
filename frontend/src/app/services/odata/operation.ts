import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ODataConfiguration } from './config';

export abstract class ODataOperation<T> {
	private _expand: string;
	private _select: string;

	constructor(protected _typeName: string,
	            protected config: ODataConfiguration,
	            protected httpClient: HttpClient) { }

	Expand(expand: string | string[]): any {
		this._expand = this.parseStringOrStringArray(expand);
		return this;
	}

	Select(select: string | string[]): any {
		this._select = this.parseStringOrStringArray(select);
		return this;
	}

	protected getParams(): HttpParams {
		let params: HttpParams = new HttpParams();

		if (this._select && this._select.length > 0) {
			params = params.set(this.config.keys.select, this._select);
		}
		if (this._expand && this._expand.length > 0) {
			params = params.set(this.config.keys.expand, this._expand);
		}

		return params;
	}

	protected handleResponse(entity: Observable<HttpResponse<Object>>): Observable<T> {
		return entity.map(this.extractData)
			.catch((err: any, caught: Observable<T>) => {
				if (this.config.handleError) {
					this.config.handleError(err, caught);
				}
				return Observable.throw(err);
			});
	}

	protected getEntityUri(entityKey: string): string {
		return this.config.getEntityUri(entityKey, this._typeName);
	}

	protected getRequestOptions() {
		return {
			headers: this.config.requestOptions.headers,
			observe: 'response' as 'response',
			params: this.getParams()
		};
	}

	protected parseStringOrStringArray(input: string | string[]): string {
		if (input instanceof Array) {
			return input.join(',');
		}

		return input as string;
	}

	private extractData(res: HttpResponse<T>): T {
		if (res.status < 200 || res.status >= 300) {
			throw new Error('Bad response status: ' + res.status);
		}

		let entity: T = res.body;
		return entity || null;
	}
}

export abstract class OperationWithKey<T> extends ODataOperation<T> {
	constructor(protected _typeName: string,
	            protected config: ODataConfiguration,
	            protected httpClient: HttpClient,
	            protected key: string) {
		super(_typeName, config, httpClient);
	}

	abstract Exec(...args): Observable<any>;
}

export class GetOperation<T> extends OperationWithKey<T> {
	public Exec(): Observable<T> {
		return super.handleResponse(this.httpClient.get(this.getEntityUri(this.key), this.getRequestOptions()));
	}
}

// export class PostOperation<T> extends OperationWithEntity<T>{
//     public Exec():Observable<T>{    //ToDo: Check ODataV4
//         let body = JSON.stringify(this.entity);
//         return this.handleResponse(this.http.post(this.config.baseUrl + "/"+this._typeName, body, this.getRequestOptions()));
//     }
// }

// export class PatchOperation<T> extends OperationWithKeyAndEntity<T>{
//     public Exec():Observable<Response>{    //ToDo: Check ODataV4
//         let body = JSON.stringify(this.entity);
//         return this.http.patch(this.getEntityUri(this.key),body,this.getRequestOptions());
//     }
// }

// export class PutOperation<T> extends OperationWithKeyAndEntity<T>{
//     public Exec(){
//         let body = JSON.stringify(this.entity);
//         return this.handleResponse(this.http.put(this.getEntityUri(this.key),body,this.getRequestOptions()));
//     }
// }
