import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { Component, Input, Output, EventEmitter, forwardRef, ViewChild, AfterContentInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, Validator } from '@angular/forms';
import { ColorPicker } from 'primeng/primeng';

export class ColorPickerChange {
	source: ColorPickerComponent;
	value: string;
}

export const GRAY_COLOR = '#d7dee2';

export function hexToNumber(hex: string): number {
	return parseInt(((hex.indexOf('#') > -1) ? hex.substring(1) : hex), 16);
}

export function numberToHex(value: number, showOriginal?: boolean): string {
	let res = value.toString(16);

	if (showOriginal) {
		return '#' + res;
	}

	return res && res.length === 6 ? '#' + res : GRAY_COLOR;
}

@Component({
	selector: 'ct-color-picker',
	templateUrl: 'color-picker.component.html',
	providers: [
		{
			provide: NG_VALUE_ACCESSOR,
			useExisting: forwardRef(() => ColorPickerComponent),
			multi: true
		},
		{
			provide: NG_VALIDATORS,
			useExisting: forwardRef(() => ColorPickerComponent),
			multi: true
		}
	]
})

export class ColorPickerComponent implements ControlValueAccessor, Validator, AfterContentInit {
	@Input() name: string;

	@Input('ngModel')
	get bgColor(): string {
		return this._bgColor;
	};

	set bgColor(value: string) {
		this.modelValue = value;
		this._bgColor = this.isColorValid(value) ? value : this._bgColor || GRAY_COLOR;
	};

	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	@Output() change: EventEmitter<ColorPickerChange> = new EventEmitter<ColorPickerChange>();
	@ViewChild('colorPicker') colorPicker: ColorPicker;

	colorMask = ['#', /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/];
	modelValue: string;

	private _bgColor: string;
	private _disabled: boolean;
	private controlValueAccessorChangeFn: (value: any) => void = (value: any) => {};
	private onTouched: () => any = () => {};

	ngAfterContentInit() {
		this.colorPicker.updateColorSelector = this.updateColorSelector.bind(this);
	}

	updateColorSelector(): void {
		let that = this.colorPicker;
		that.colorSelectorViewChild.nativeElement.style.backgroundColor = '#' + that.HSBtoHEX({
			h: that.value.h,
			s: 100,
			b: 100
		});
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(value: string) {
		this.modelValue = value;
		this.emitChangeEvent();

		if (this.isColorValid(value)) {
			this.bgColor = value;
		}
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnChange(fn: (value: any) => void) {
		this.controlValueAccessorChangeFn = fn;
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

	/**
	 * Implemented as part of Validator.
	 */
	validate(c: FormControl): { [key: string]: any } {
		return (this.isColorValid(this.modelValue)) ? null : {
			colorInvalid: true
		};
	}

	private isColorValid(value: string): boolean {
		return !!value && /^#?([a-fA-F0-9]{6})$/.test(value);
	}

	private emitChangeEvent() {
		let event = new ColorPickerChange();
		event.source = this;
		event.value = this.modelValue;

		this.controlValueAccessorChangeFn(this.modelValue);
		this.change.emit(event);
	}
}
