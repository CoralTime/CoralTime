import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ODataConfiguration } from './config';
import { ODataQuery } from './query';
import { NotificationService } from '../../core/notification.service';

export class ODataService<T> {
	constructor(private _typeName: string,
	            private http: HttpClient,
	            private config: ODataConfiguration,
	            private notificationService: NotificationService) { }

	get TypeName(): string {
		return this._typeName;
	}

	Post(entity: T): Observable<T> {
		let body = JSON.stringify(entity);
		return this.handleResponse(this.http.post<T>(this.config.baseUrl + '/' + this.TypeName, body, this.config.postRequestOptions));
	}

	Patch(entity: any, key: string): Observable<Object> {
		let body = JSON.stringify(entity);
		return this.http.patch(this.getEntityUri(key), body, this.config.postRequestOptions);
	}

	Put(entity: T, key: string): Observable<T> {
		let body = JSON.stringify(entity);
		return this.handleResponse(this.http.put<T>(this.getEntityUri(key), body, this.config.postRequestOptions));
	}

	Delete(key: string): Observable<T> {
		return this.http.delete<T>(this.getEntityUri(key), this.config.requestOptions);
	}

	Query(): ODataQuery<T> {
		return new ODataQuery<T>(this.TypeName, this.config, this.http, this.notificationService);
	}

	protected getEntityUri(entityKey: string): string {
		return this.config.getEntityUri(entityKey, this._typeName);
	}

	protected handleResponse(entity: Observable<HttpResponse<T>>): Observable<T> {
		return entity.map(this.extractData)
			.catch((err: any, caught: Observable<T>) => {
				if (this.config.handleError) {
					this.config.handleError(err, caught);
				}
				return Observable.throw(err);
			});
	}

	private extractData(res: HttpResponse<T>): T {
		if (res.status < 200 || res.status >= 300) {
			throw new Error('Bad response status: ' + res.status);
		}

		let entity: T = res.body;
		return entity || null;
	}
}
