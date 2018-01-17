import { Directive, ElementRef, Output, EventEmitter, Input, Renderer2, OnChanges, OnDestroy } from '@angular/core';

@Directive({
	selector: '[clickOutside]'
})

export class ClickOutsideDirective implements OnChanges, OnDestroy {
	@Input() canClickOverlay: boolean = true;
	@Input() isOpen: boolean = true;

	@Output() clickOutside = new EventEmitter<MouseEvent>();

	private documentClickListener: any;

	constructor(private _elementRef: ElementRef, public renderer: Renderer2) {
		this.bindDocumentClickListener();
	}

	ngOnChanges() {
		if (this.isOpen) {
			this.bindDocumentClickListener();
		} else {
			this.unbindDocumentClickListener();
		}
	}

	bindDocumentClickListener(): void {
		if (!this.documentClickListener) {
			this.documentClickListener = this.renderer.listen('document', 'mousedown', (event: MouseEvent) => {
				let targetElement = <HTMLElement>event.target;

				if (!targetElement || !this.isOpen) {
					return;
				}

				let overlayElement = document.getElementsByClassName('cdk-overlay-container')[0];
				const clickedOverlay = overlayElement ? overlayElement.contains(targetElement) : false;
				if (this.canClickOverlay) {
					if (clickedOverlay) {
						return;
					}
				}

				const clickedMatRippleButton = targetElement.classList.contains('mat-button-ripple');
				const clickedDatepicker = targetElement.classList.contains('dp-calendar-day') || targetElement.classList.contains('dp-calendar-month');
				const clickedInside = this._elementRef.nativeElement.contains(targetElement);
				if (!clickedInside && !clickedDatepicker && !clickedMatRippleButton) {
					this.clickOutside.emit(event);
				}
			});
		}
	}

	unbindDocumentClickListener(): void {
		if (this.documentClickListener) {
			this.documentClickListener();
			this.documentClickListener = null;
		}
	}

	ngOnDestroy() {
		this.unbindDocumentClickListener();
	}
}
