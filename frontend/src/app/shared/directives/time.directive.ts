import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';

@Directive({
	selector: '[ctTime]'
})

export class TimeDirective {
	@Input() ctTime: number;
	@Input() enableFormat: boolean = true;
	@Output() ngModelChange: EventEmitter<any> = new EventEmitter();
	@Output() timeChanged: EventEmitter<any> = new EventEmitter();

	private oldValue: string = '';

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let current: string = this.el.nativeElement.value;
		let index = this.ctTime === 24 ? 1 : 5;

		switch (event.key) {
			case 'ArrowDown' :
				current = current || this.oldValue;
				current = +current > 0 ? String(+current - index) : current;
				break;
			case 'ArrowUp' :
				current = current || this.oldValue;
				current = +current + index < this.ctTime ? String(+current + index) : current;
				break;
		}

		current = this.limitTime(current);
		if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
			current = this.formatTime(current);
		}

		if (event.key !== 'ArrowLeft' && event.key !== 'ArrowRight' && event.key !== 'Tab') {
			this.timeChanged.emit(current);
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
		if (this.enableFormat) {
			return (+time >= 0 && +time < 10) ? '0' + +time : time;
		} else {
			return +time + '';
		}
	}

	private limitTime(time: string): string {
		if (+time < 0) {
			time = '0';
		}
		if (+time >= this.ctTime) {
			time = this.ctTime - 1 + '';
		}

		return time;
	}
}
