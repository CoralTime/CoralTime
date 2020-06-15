import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { LoadingMaskService } from './loading-mask.service';
import { ImpersonationService } from '../../services/impersonation.service';

@Component({
	selector: 'ct-loading-mask',
	templateUrl: 'loading-mask.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})

export class LoadingMaskComponent implements OnInit {
	showMask: boolean;

	constructor(public impersonationService: ImpersonationService,
	            private ref: ChangeDetectorRef,
	            private service: LoadingMaskService) {
	}

	ngOnInit(): void {
		this.service.onChange.subscribe((showMask) => {
			this.showMask = showMask;
			this.ref.markForCheck();
		});
	}
}
