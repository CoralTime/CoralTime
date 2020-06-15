import { LoadingBarService } from '@ngx-loading-bar/core';
import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { _finally } from 'rxjs/operator/finally';

@Injectable()
export class LoadingBarInterceptor implements HttpInterceptor {
	private loadingService: LoadingBarService;

	constructor(private injector: Injector) {
	}

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		if (!this.loadingService) {
			this.loadingService = this.injector.get(LoadingBarService);
		}

		const r = next.handle(req);

		let started = false;
		const responseSubscribe = r.subscribe.bind(r);
		r.subscribe = (...args) => {
			this.loadingService.start();
			started = true;
			return responseSubscribe(...args);
		};

		return _finally.call(r, () => started && this.loadingService.complete());
	}
}
