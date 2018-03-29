import { Subscription } from 'rxjs';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { LoadingIndicatorService } from '../../core/loading-indicator.service';

@Component({
	selector: 'ct-loading-bar',
	templateUrl: 'loading-bar.component.html'
})

export class LoadingBarComponent implements OnInit, OnDestroy {
	status = 0;
	private subscription: Subscription;

	constructor(private indicatorService: LoadingIndicatorService) {
	}

	ngOnInit() {
		this.subscription = this.indicatorService.getStatus().subscribe(status => {
			setTimeout(() => {
				this.status = +status;
			}, 0);
		});
	}

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}
}
