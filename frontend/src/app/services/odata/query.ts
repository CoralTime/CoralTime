import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ODataConfiguration } from './config';
import { ODataOperation } from './operation';
import { NotificationService } from '../../core/notification.service';

export class PagedResult<T> {
	count: number;
	data: T[];
}

export class ODataQuery<T> extends ODataOperation<T> {
	private _filter: string;
	private _top: number;
	private _skip: number;
	private _orderBy: string;
	private _property: string;

	constructor(_typeName: string,
	            config: ODataConfiguration,
	            httpClient: HttpClient,
	            private notificationService: NotificationService) {
		super(_typeName, config, httpClient);
	}

	GetProperty(property: string): any {
		this._property = property;
		return this;
	}

	Filter(filter: string): ODataQuery<T> {
		this._filter = filter;
		return this;
	}

	OrderBy(orderBy: string): ODataQuery<T> {
		this._orderBy = orderBy;
		return this;
	}

	Skip(skip: number): ODataQuery<T> {
		this._skip = skip;
		return this;
	}

	Top(top: number): ODataQuery<T> {
		this._top = top;
		return this;
	}

	Exec(): Observable<Array<T>> {
		let params = this.getQueryParams();
		let config = this.config;

		return this.httpClient.get(this.buildResourceURL(), {params: params})
			.map(res => this.extractArrayData(res, config))
			.catch((err: Response, caught: Observable<Array<T>>) => {
				if (this.config.handleError) {
					if (err.status === 500) {
						this.notificationService.danger('Server error. Try again later.');
					}
					this.config.handleError(err, caught);
				}

				return Observable.throw(err);
			});
	}

	ExecWithCount(): Observable<PagedResult<T>> {
		let params = this.getQueryParams()
			.set('$count', 'true'); // OData v4 only
		let config = this.config;

		return this.httpClient.get<HttpResponse<T>>(this.buildResourceURL(), {params: params})
			.map(res => this.extractArrayDataWithCount(res, config))
			.catch((err: any, caught: Observable<PagedResult<T>>) => {
				if (this.config.handleError) {
					if (err.status === 500) {
						this.notificationService.danger('Server error. Try again later.');
					}
					this.config.handleError(err, caught);
				}

				return Observable.throw(err);
			});
	}

	private buildResourceURL(): string {
		this._property = this._property ? '/' + this._property : '';
		return this.config.baseUrl + '/' + this._typeName + this._property + '()';
	}

	getQueryParams(): HttpParams {
		let params = super.getParams();

		if (this._filter) {
			params = params.set(this.config.keys.filter, this._filter);
		}
		if (this._top) {
			params = params.set(this.config.keys.top, this._top.toString());
		}
		if (this._skip) {
			params = params.set(this.config.keys.skip, this._skip.toString());
		}
		if (this._orderBy) {
			params = params.set(this.config.keys.orderBy, this._orderBy);
		}

		return params;
	}

	private extractArrayData(res: any, config: ODataConfiguration): Array<T> {
		return config.extractQueryResultData<T>(res);
	}

	private extractArrayDataWithCount(res: HttpResponse<T>, config: ODataConfiguration): PagedResult<T> {
		return config.extractQueryResultDataWithCount<T>(res);
	}
}
