import {
	Component, Input, Output, EventEmitter, forwardRef,
	ChangeDetectionStrategy, ChangeDetectorRef, ViewChild
} from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { MatTextareaAutosize } from '@angular/material';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

export const TEXTAREA_CONTROL_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => TextareaComponent),
	multi: true
};

export class TextareaChange {
	source: TextareaComponent;
	value: any[];
}

@Component({
	selector: 'ct-textarea',
	templateUrl: 'textarea.component.html',
	providers: [TEXTAREA_CONTROL_VALUE_ACCESSOR],
	changeDetection: ChangeDetectionStrategy.OnPush
})

export class TextareaComponent implements ControlValueAccessor {
	@Input() name: string;
	@Input() maxlength: number;
	@Input() placeholder: string = '';
	@Input() canResize: boolean = false;

	@Output() change: EventEmitter<TextareaChange> = new EventEmitter<TextareaChange>();

	@ViewChild('slimScroll') slimScroll: any;
	@ViewChild('autoSizer') autoSizer: MatTextareaAutosize;

	isFocusClassShown: boolean;
	modelValue: any;

	private _disabled: boolean;

	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private _controlValueAccessorChangeFn: (value: any) => void = () => {};
	private onTouched: () => any = () => {};

	constructor(private ref: ChangeDetectorRef) {
		setTimeout(() => {
			this.autoSizer.resizeToFitContent(true);
		}, 0);
	}

	updateModel(modelValue: any) {
		this.modelValue = modelValue;
		this._emitChangeEvent();
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(value: any) {
		this.modelValue = value;
		this.ref.markForCheck();
		if (this.modelValue) {
			this._controlValueAccessorChangeFn(this.modelValue);
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

	private _emitChangeEvent(): void {
		let event = new TextareaChange();
		event.source = this;
		event.value = this.modelValue;

		this._controlValueAccessorChangeFn(this.modelValue);
		this.resizeTextarea();
		this.change.emit(event);
	}

	private resizeTextarea(): void {
		this.autoSizer.resizeToFitContent(true);
		this.slimScroll.getBarHeight();
	}
}
