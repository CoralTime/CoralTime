import { Directive, ElementRef, Input, HostListener, Renderer, Output, EventEmitter } from '@angular/core';
import { LIST_ITEM_HEIGHT } from '../form/select/select.component';

@Directive({
	selector: '[ctSlimScrollArrows][slimScroll]'
})

export class SlimscrollArrowsDirective {
	@Input('ctSlimScrollArrows') slimScroll: any;

	@Output() scroll = new EventEmitter();

	private interval: any;
	private parent: Node = this.el.nativeElement.parentNode;

	constructor(private el: ElementRef,
	            private renderer: Renderer) {
	}

	private clearInterval(): void {
		clearInterval(this.interval);
		this.interval = null;
	}

	private scrollContent(y: number, isWheel: boolean, isJump: boolean): void {
		let context = this.slimScroll;
		let delta = y;
		let maxTop = context.el.offsetHeight - context.bar.offsetHeight;
		let bar = context.bar;
		let el = context.el;

		if (isJump) {
			el.scrollTop = el.scrollTop + LIST_ITEM_HEIGHT * y;
			delta = el.scrollTop / (el.scrollHeight - el.offsetHeight) * (el.offsetHeight - bar.offsetHeight);
		}

		delta = Math.min(Math.max(delta, 0), maxTop);
		this.renderer.setElementStyle(bar, 'top', delta + 'px');
		this.scroll.emit();
	}

	@HostListener('document:mousedown', ['$event', '$event.target'])
	onMouseDown(event: MouseEvent, targetElement: HTMLElement) {
		if (!targetElement || (this.parent && !this.parent.contains(targetElement)) || this.interval) {
			return;
		}

		if (targetElement.classList.contains('slimscroll-grid')) {
			this.interval = setInterval(() => {
				if (event.clientY > this.slimScroll.bar.getBoundingClientRect().bottom) {
					this.scrollContent(1, false, true);
				}

				if (event.clientY < this.slimScroll.bar.getBoundingClientRect().top) {
					this.scrollContent(-1, false, true);
				}
			}, 25);
		}
	}

	@HostListener('document:mouseup')
	onMouseUp() {
		if (this.interval) {
			this.clearInterval();
		}
	}
}
