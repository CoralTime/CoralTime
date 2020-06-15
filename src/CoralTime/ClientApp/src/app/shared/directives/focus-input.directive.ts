import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
	selector: '[ctFocus]'
})

export class FocusInputDirective {
	@Input() ctFocus: HTMLInputElement;

	constructor(private el: ElementRef) {
	}

	@HostListener('keyup', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let current: string = this.el.nativeElement.value;

		if (this.ctFocus && this.isNumber(event.key) && current.trim().length === 2) {
			this.ctFocus.focus();
		}
	}

	private isNumber(value: string): boolean {
		return value >= '0' && value <= '9';
	}
}
