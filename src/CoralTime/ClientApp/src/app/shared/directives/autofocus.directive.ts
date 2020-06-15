import { Directive, ElementRef, AfterViewInit } from '@angular/core';

@Directive({
	selector: '[autofocus]'
})

export class FocusDirective implements AfterViewInit {
	constructor(private element: ElementRef) {
	}

	ngAfterViewInit() {
		setTimeout(() => {
			this.element.nativeElement.select();
		}, 500);
	}
}
