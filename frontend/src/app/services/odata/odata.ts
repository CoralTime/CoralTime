import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { ODataConfiguration } from './config';
import { ODataQuery } from './query';
import { GetOperation } from './operation';
import { NotificationService } from '../../core/notification.service';

export class ODataService<T> {
	constructor(private _typeName: string,
	            private http: Http,
	            private config: ODataConfiguration,
	            private notificationService: NotificationService) { }

	get TypeName(): string {
		return this._typeName;
	}

	Get(key: string): GetOperation<T> {
		return new GetOperation<T>(this._typeName, this.config, this.http, key);
	}

	Post(entity: T): Observable<T> {
		let body = JSON.stringify(entity);
		return this.handleResponse(this.http.post(this.config.baseUrl + '/' + this.TypeName, body, this.config.postRequestOptions));
	}

	CustomAction(key: string, actionName: string, postdata: any): Observable<any> {
		let body = JSON.stringify(postdata);
		return this.http.post(this.getEntityUri(key) + '/' + actionName, body, this.config.requestOptions).map(resp => resp.json());
	}

	CustomFunction(key: string, actionName: string): Observable<any> {
		return this.http.get(this.getEntityUri(key) + '/' + actionName, this.config.requestOptions).map(resp => resp.json());
	}

	Patch(entity: any, key: string): Observable<Response> {
		let body = JSON.stringify(entity);
		return this.http.patch(this.getEntityUri(key), body, this.config.postRequestOptions);
	}

	Put(entity: T, key: string): Observable<T> {
		let body = JSON.stringify(entity);
		return this.handleResponse(this.http.put(this.getEntityUri(key), body, this.config.postRequestOptions));
	}

	Delete(key: string): Observable<Response> {
		return this.http.delete(this.getEntityUri(key), this.config.requestOptions);
	}

	Query(): ODataQuery<T> {
		return new ODataQuery<T>(this.TypeName, this.config, this.http, this.notificationService);
	}

	protected getEntityUri(entityKey: string): string {
		return this.config.getEntityUri(entityKey, this._typeName);
	}

	protected handleResponse(entity: Observable<Response>): Observable<T> {
		return entity.map(this.extractData)
			.catch((err: any, caught: Observable<T>) => {
				if (this.config.handleError) {
					this.config.handleError(err, caught);
				}
				return Observable.throw(err);
			});
	}

	private extractData(res: Response): T {
		if (res.status < 200 || res.status >= 300) {
			throw new Error('Bad response status: ' + res.status);
		}
		let body: any = res.json();
		let entity: T = body;
		return entity || null;
	}
}
