import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { isNumber } from 'util';

@Component({
	selector: 'ct-show-status',
	template: `
        <span *ngIf="status" class="ct-result-message ct-{{status}}">
			<i [class]="getIconClass()"></i>
			{{getMessage()}}
		</span>
	`
})

export class ShowStatusComponent implements OnChanges {
	@Input() status: 'success' | 'error' | 'loading';
	private removeStatusElementTimeout: any;

	ngOnChanges(changes: SimpleChanges) {
		if (changes && changes['status']) {
			if (!this.status) {
				return;
			}

			this.removeStatusElement(this.status === 'success' ? 5000  : null);
		}
	}

	getIconClass(): string {
		if (this.status === 'success') {
			return 'fa fa-check';
		}
		if (this.status === 'error') {
			return 'fa fa-times';
		}
		return '';
	}

	getMessage(): string {
		if (this.status === 'success') {
			return 'Saved';
		}
		if (this.status === 'error') {
			return 'Error';
		}

		return '';
	}

	removeStatusElement(timer?: number): void {
		clearTimeout(this.removeStatusElementTimeout);
		if (!this.status || !isNumber(timer)) {
			return;
		}

		if (timer === 0) {
			this.status = null;
			return;
		}

		this.removeStatusElementTimeout = setTimeout(() => {
			this.status = null;
		}, timer)
	}
}
