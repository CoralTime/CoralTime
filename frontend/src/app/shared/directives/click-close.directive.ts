import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
	selector: '[ctClickClose]'
})

export class ClickCloseDirective {
	constructor(private el: ElementRef) {
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (event.key === 'Escape') {
			this.el.nativeElement.click();
		}
	}
}
