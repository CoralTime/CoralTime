import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';

@Directive({
	selector: '[ctTime]'
})

export class TimeDirective {
	@Input() ctTime: number;
	@Input() min: number = 0;
	@Input() step: number = 1;
	@Input() enableFormat: boolean = true;
	@Output() ngModelChange: EventEmitter<any> = new EventEmitter();
	@Output() timeChanged: EventEmitter<any> = new EventEmitter();

	private oldValue: string = '';

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let current: string;
		let time: number;

		switch (event.key) {
			case 'ArrowDown' :
				current = this.el.nativeElement.value.trim();
				time = (current.length == 0) ? this.ctTime : +current - this.step;
				if (time < this.min) {
					time = this.ctTime;
				}
				this.processChange(time);   
				break;
			case 'ArrowUp' :
				current = this.el.nativeElement.value.trim();        
				time = (current.length == 0) ? this.min : +current + this.step;
				if (time > this.ctTime) {
					time = this.min;
				}
				this.processChange(time);
				break;
			case 'Backspace' :
			case 'Delete' :
				this.el.nativeElement.value = '';
				break;
			default:
				break;
		}
	}

	@HostListener('keypress', ['$event'])
	onKeyPress(event: KeyboardEvent) {
		//"keypress" & "beforeinput" essentially do the same thing however we need both for this to work on all browsers.
		//Chrome on Android does not support "keypress".
		//MS Edge & Firefox on desktops do not support "beforeinput".
		switch (event.key) {
			case 'Enter' :
				return true;
			default:
				return this.handleBeforeInput(event.key);
		}
	}

	@HostListener('beforeinput', ['$event'])
	onBeforeInput(event: InputEvent) {
		return this.handleBeforeInput(event.data);
	}
   
	@HostListener('input', ['$event'])
	onInput(event: InputEvent) {
		this.handleInput(this.el.nativeElement.value)
	}

	@HostListener('focus')
	onFocus() {
		this.oldValue = this.el.nativeElement.value;
		this.el.nativeElement.setSelectionRange(0, 0); 
	}

	@HostListener('blur')
	onBlur() {
		let current = this.el.nativeElement.value.trim();
		let time = (current.length == 0) ? +this.oldValue : +current;
		this.processChange(time);
	}

	private handleBeforeInput(data: string): boolean {
		switch (data) {
			case '0' :
			case '1' :
			case '2' :
			case '3' :
			case '4' :
			case '5' :
			case '6' :
			case '7' :
			case '8' :
			case '9' :
				if (this.el.nativeElement.value.trim().length == 2) {
					this.el.nativeElement.value = data;
					this.handleInput(data);
					return false;
				}
				return true;
			default:
				return false;
		}
	}

	private handleInput(data: string): void {
		var current = data.trim();
		if (current.length == 1) {
			let nextPossible = +(current + '0');
			if (nextPossible > this.ctTime) {
				this.processChange(+current);
			}
		}
		else if (current.length == 2) {
			this.processChange(+current);
		}
	}

	private formatTime(time: number): string {
		if (this.enableFormat) {
			return (time >= 0 && time < 10) ? '0' + time : time + '';
		} else {
			return time + '';
		}
	}

	private limitTime(time: number): number {
		if (time < this.min) {
			return this.min;
		}
		else if (time > this.ctTime) {
			return this.ctTime;
		}
		else {
			return time - (time % this.step);
		}
	}

	private processChange(time: number): void {
		let current: string = this.formatTime(this.limitTime(time));
		this.ngModelChange.emit(current);
		this.timeChanged.emit(current);
	}
}
