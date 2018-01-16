import { Directive, ElementRef, Input, OnChanges } from '@angular/core';

@Directive({
	selector: '[disableWhenRequest][disabled]'
})

export class DisableWhenRequestDirective implements OnChanges {
	@Input() disableWhenRequest: Promise<any>;

	constructor(private el: ElementRef) {
	}

	ngOnChanges(changes) {
		if (changes && changes.disableWhenRequest && changes.disableWhenRequest.currentValue) {
			this.disableButton(this.disableWhenRequest);
		}
	}

	disableButton(disableWhenRequest: Promise<any>) {
		this.el.nativeElement.disabled = true;
		disableWhenRequest.then(() => this.el.nativeElement.disabled = false);
	}
}
