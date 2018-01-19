import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({
	selector: '[time]'
})

export class TimeDirective {
	@Output() ngModelChange: EventEmitter<any> = new EventEmitter();

	private oldValue: string;

	constructor(private el: ElementRef) {
	}

	@HostListener('keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		let time: string = this.el.nativeElement.value;

		switch (event.key) {
			case 'ArrowDown' :
				time = this.convertTimeToString(this.convertTimeToMinutes(time) - 5);
				break;
			case 'ArrowUp' :
				time = this.convertTimeToString(this.convertTimeToMinutes(time) + 5);
				break;
		}

		if (event.key !== 'ArrowLeft' && event.key !== 'ArrowRight' && event.key !== 'Tab') {
			this.ngModelChange.emit(time);
		}
	}

	@HostListener('focus')
	onFocus() {
		this.el.nativeElement.select();
		this.oldValue = this.el.nativeElement.value;
	}

	@HostListener('blur')
	onBlur() {
		let time: string = this.el.nativeElement.value;
		time = this.convertTimeToString(this.convertTimeToMinutes(time));

		if (time !== this.oldValue) {
			this.ngModelChange.emit(time);
		}
	}

	private convertTimeToMinutes(time: string): number {
		let arr = time.split(':');
		return (+arr[0] || 0) * 60 + (+arr[1] || 0);
	}

	private convertTimeToString(time: number): string {
		time = this.limitTime(time);
		let h = Math.floor(time / 60);
		let m = time % 60;

		return ('00' + h).slice(-2) + ':' + ('00' + m).slice(-2);
	}

	private limitTime(time: number): number {
		if (time < 0) {
			time = 0;
		}
		if (time >= 60 * 24) {
			time = 60 * 24 - 1;
		}

		return time;
	}
}
