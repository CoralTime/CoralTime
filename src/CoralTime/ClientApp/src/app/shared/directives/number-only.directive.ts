import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
	selector: '[numberOnly]'
})

export class NumberOnlyDirective {
	private regex: RegExp = new RegExp(/^[0-9]+$/g);
	private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'ArrowDown', 'ArrowUp'];

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (this.specialKeys.indexOf(event.key) !== -1) {
			return;
		}

		let current: string = this.el.nativeElement.value;
		// We need this because the current value on the DOM element
		// is not yet updated with the value from this event
		let next: string = current.concat(event.key);
		if (next && !String(next).match(this.regex)) {
			event.preventDefault();
		}
	}
}
