import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';

@Directive({
	selector: '[time]',
	host: {
		'(ngModelChange)': 'onInputChange($event)'
	}
})

export class TimeDirective {
	@Input() time: number;
	@Output() ngModelChange: EventEmitter<any> = new EventEmitter();

	private valueChanged: boolean = false;
	private oldValue: string = '';

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let current: string = this.el.nativeElement.value;

		switch (event.key) {
			case 'Tab' :
				this.valueChanged = current && current !== this.oldValue;
				return;
			case 'ArrowDown' :
				current = current ? String(+current - 1) : String(+this.oldValue - 1);
				break;
			case 'ArrowUp' :
				current = current ? String(+current + 1) : String(+this.oldValue + 1);
				break;
		}

		current = this.limitTime(current);
		if (event.key == 'ArrowDown' || event.key == 'ArrowUp') {
			current = this.formatTime(current);
		}

		this.ngModelChange.emit(current);
	}

	@HostListener('focus')
	onFocus() {
		this.valueChanged = false;
		this.oldValue = this.el.nativeElement.value;
		this.el.nativeElement.value = '';
	}

	@HostListener('blur')
	onBlur() {
		let time: string = this.el.nativeElement.value;
		if (!this.valueChanged) {
			time = this.oldValue;
			this.el.nativeElement.value = this.oldValue;
		}
		time = time ? this.formatTime(this.limitTime(time)) : '00';

		this.ngModelChange.emit(time);
	}

	onInputChange(value: string): void {
		if (isNaN(+value)) {
			this.el.nativeElement.value = this.oldValue;
		}
		this.valueChanged = true;
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