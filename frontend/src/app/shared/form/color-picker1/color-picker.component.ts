import { Component, Input, Output, EventEmitter, forwardRef, HostListener, ViewChild, AfterContentInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, Validator } from '@angular/forms';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { ColorPicker } from 'primeng/primeng';

export class ColorPickerChange {
	source: ColorPickerComponent;
	value: string;
}

export function numberToHex(value: number): string {
	return value.toString(16);
}

export function hexToNumber(hex: string): number {
	return parseInt(((hex.indexOf('#') > -1) ? hex.substring(1) : hex), 16);
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
			multi: true,
		}
	]
})

export class ColorPickerComponent implements ControlValueAccessor, AfterContentInit, Validator {
	@Input() name: string;

	@Output() change: EventEmitter<ColorPickerChange> = new EventEmitter<ColorPickerChange>();

	@ViewChild('colorPicker') colorPicker: ColorPicker;
	@ViewChild('input') input;

	colorMask = ['#', /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/, /[a-fA-F0-9]/];
	modelValue: string;

	private _disabled: boolean = false;

	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private controlValueAccessorChangeFn: (value: any) => void = () => {};
	private onTouched: () => any = () => {};

	ngAfterContentInit() {
		this.colorPicker.defaultColor = '#d7dee2';
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(selectedColor: string) {
		this.colorPicker.value = selectedColor;
		this.modelValue = selectedColor;
		this.emitChangeEvent();
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnChange(fn: (value: any) => void) {
		this.controlValueAccessorChangeFn = fn;
	}

	/**
	 * Implemented as part of Validator.
	 */
	validate(c: FormControl): { [key: string]: any } {
		return (this.input.valid) ? null : {
			colorInvalid: true
		};
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

	private emitChangeEvent() {
		let event = new ColorPickerChange();
		event.source = this;
		event.value = this.modelValue;

		this.controlValueAccessorChangeFn(this.modelValue);
		this.change.emit(event);
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent): void {
		if (!this.colorPicker.panelVisible) {
			return;
		}

		event.preventDefault();
		event.stopPropagation();

		if (event.key === 'Enter') {
			this.colorPicker.hide();
			return;
		}
	}
}
