import { NgForOf } from '@angular/common';
import { Directive, Input, OnChanges, SimpleChange } from '@angular/core';

@Directive({
	selector: '[ngFor][ngForIn]'
})

export class NgForIn<T> extends NgForOf<T> implements OnChanges {
	@Input() ngForIn: any;

	ngOnChanges(changes: any) {
		if (changes.ngForIn) {
			this.ngForOf = Object.keys(this.ngForIn) as Array<any>;

			const change = changes.ngForIn;
			const currentValue = Object.keys(change.currentValue);
			const previousValue = change.previousValue ? Object.keys(change.previousValue) : undefined;
			changes.ngForOf = new SimpleChange(previousValue, currentValue, change.firstChange);

			super.ngOnChanges(changes);
		}
	}
}
