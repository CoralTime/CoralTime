import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
	selector: 'ct-loading-bar',
	templateUrl: 'loading-bar.component.html'
})

export class LoadingBarComponent implements OnInit, OnDestroy {
	status = 0;
	private subscription: Subscription;

	constructor(public loader: LoadingBarService) {
	}

	ngOnInit() {
		this.subscription = this.loader.progress$.subscribe((status: number) => {
				this.status = status;
		});
	}

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}
}
