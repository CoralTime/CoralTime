import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Component, Input, Output, EventEmitter, forwardRef, HostListener } from '@angular/core';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

export const COLOR_PICKER_CONTROL_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => ColorPickerComponent),
	multi: true
};

export class ColorPickerChange {
	source: ColorPickerComponent;
	value: number;
}

@Component({
	selector: 'ct-color-picker',
	templateUrl: 'color-picker.component.html',
	providers: [COLOR_PICKER_CONTROL_VALUE_ACCESSOR]
})

export class ColorPickerComponent implements ControlValueAccessor {
	@Input('canClickOverlay') canClickOverlay: boolean = false;
	@Input('name') name: string;
	@Input('options') options: any[];

	@Output() change: EventEmitter<ColorPickerChange> = new EventEmitter<ColorPickerChange>();

	isOpen: boolean = false;
	oldSelectedColor: number;
	selectedColor: number;

	private _disabled: boolean = false;

	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private _controlValueAccessorChangeFn: (value: any) => void = () => {};
	private onTouched: () => any = () => {};

	constructor() {
	}

	isColorSelected(color: number): boolean {
		return this.selectedColor === color;
	}

	selectColor(option: any) {
		this.selectedColor = option;
		this.onTouched();
		this.closeControl();

		if (option !== this.oldSelectedColor) {
			this._emitChangeEvent();
		}
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(selectedColor: any = null) {
		this.selectedColor = selectedColor;
		if (this.selectedColor) {
			this._controlValueAccessorChangeFn(this.selectedColor);
		}
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnChange(fn: (value: any) => void) {
		this._controlValueAccessorChangeFn = fn;
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnTouched(fn: any) {
		this.onTouched = fn;
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	setDisabledState(isDisabled: boolean) {
		this.disabled = isDisabled;
	}

	closeControl(): void {
		this.isOpen = false;
	}

	openControl(): void {
		this.isOpen = true;
		this.oldSelectedColor = this.selectedColor;
	}

	toggleControl(): void {
		if (!this._disabled) {
			if (this.isOpen) {
				this.closeControl();
			} else {
				this.openControl();
			}
		}
	}

	private _emitChangeEvent() {
		let event = new ColorPickerChange();
		event.source = this;
		event.value = this.selectedColor;

		this._controlValueAccessorChangeFn(this.selectedColor);
		this.change.emit(event);
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent): void {
		if (!this.isOpen) {
			return;
		}

		event.preventDefault();
		event.stopPropagation();

		let maxValue = this.options.length - 1;
		let halfOfMaxValue = Math.ceil(maxValue / 2);

		if (event.key === 'ArrowDown') {
			this.selectedColor = this.selectedColor < halfOfMaxValue ? this.selectedColor + halfOfMaxValue : this.selectedColor;
			return;
		}

		if (event.key === 'ArrowUp') {
			this.selectedColor = +this.selectedColor >= halfOfMaxValue ? this.selectedColor - halfOfMaxValue : this.selectedColor;
			return;
		}

		if (event.key === 'ArrowLeft') {
			this.selectedColor = Math.max(+this.selectedColor - 1, 0);
			return;
		}

		if (event.key === 'ArrowRight') {
			this.selectedColor = Math.min(+this.selectedColor + 1, maxValue);
			return;
		}

		if (event.key === 'Enter') {
			this.toggleControl();
			return;
		}
	}
}
