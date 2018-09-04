import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';

@Directive({
	selector: '[time]'
})

export class TimeDirective {
	@Input() time: number;
	@Output() ngModelChange: EventEmitter<any> = new EventEmitter();
	@Output() timeChanged: EventEmitter<any> = new EventEmitter();

	private oldValue: string = '';

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let current: string = this.el.nativeElement.value;
		let index = this.time === 24 ? 1 : 5;

		switch (event.key) {
			case 'ArrowDown' :
				current = current || this.oldValue;
				current = +current > 0 ? String(+current - index) : current;
				break;
			case 'ArrowUp' :
				current = current || this.oldValue;
				current = +current + index < this.time ? String(+current + index) : current;
				break;
		}

		current = this.limitTime(current);
		if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
			current = this.formatTime(current);
		}

		this.ngModelChange.emit(current);
	}

	@HostListener('focus')
	onFocus() {
		this.oldValue = this.el.nativeElement.value;
		this.el.nativeElement.value = '';
	}

	@HostListener('blur')
	onBlur() {
		let time: string = this.el.nativeElement.value;

		if (!time || time === this.oldValue) {
			time = this.oldValue;
			this.el.nativeElement.value = this.oldValue;
			this.ngModelChange.emit(time);
		} else {
			time = time ? this.formatTime(this.limitTime(time)) : '00';
			this.ngModelChange.emit(time);
			this.timeChanged.emit(time);
		}
	}

	private formatTime(time?: string): string {
		return (+time >= 0 && +time < 10) ? '0' + +time : time;
	}

	private limitTime(time: string): string {
		if (+time < 0) {
			time = '0';
		}
		if (+time >= this.time) {
			time = this.time - 1 + '';
		}

		return time;
	}
}
