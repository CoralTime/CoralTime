import { Directive, TemplateRef, ViewContainerRef, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoadingIndicatorService } from '../../core/loading-indicator.service';

@Directive({selector: '[ctLoadingIndicator]'})
export class LoadingIndicatorDirective implements OnInit, OnDestroy {
	private status = 0;
	private subscription: Subscription;

	constructor(private templateRef: TemplateRef<any>,
	            private viewContainer: ViewContainerRef,
	            private indicatorService: LoadingIndicatorService,
	            private ref: ChangeDetectorRef) { }

	ngOnInit() {
		this.process(0);
		this.subscription = this.indicatorService.getStatus().subscribe(status => this.process(status));
	}

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}

	private process(status: number | {}) {
		setTimeout(() => {
			if (status > 0 && this.status === 0) {
				this.viewContainer.createEmbeddedView(this.templateRef);
			} else if (status === 0 && this.status > 0) {
				this.viewContainer.clear();
			}
			this.status = +status;
			this.ref.markForCheck();
		}, 0);
	}
}
