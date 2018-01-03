import { LoadingIndicatorService } from './loading-indicator.service';
import { Injectable } from '@angular/core';
import { BrowserXhr } from '@angular/http';

@Injectable()
export class CustomBrowserXhr extends BrowserXhr {
	private reqsTotal: number = 0;
	private reqsCompleted = 0;

	constructor(private indicatorService: LoadingIndicatorService) {
		super();
	}

	build(): any {
		let xhr = super.build();

		xhr.onloadstart = (event) => {
			if (this.reqsTotal === 0) {
				this.indicatorService.start();
			}
			this.reqsTotal++;
			this.indicatorService.set(this.reqsCompleted / this.reqsTotal);
		};

		xhr.onloadend = (event) => {
			this.reqsCompleted++;
			if (this.reqsCompleted >= this.reqsTotal) {
				this.setComplete();
			} else {
				this.indicatorService.set(this.reqsCompleted / this.reqsTotal);
			}
		};

		return <any>(xhr);
	}

	private setComplete(): void {
		this.indicatorService.complete();
		this.reqsCompleted = 0;
		this.reqsTotal = 0;
	}
}