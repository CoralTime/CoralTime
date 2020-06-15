import {
	Component, Input, Output, EventEmitter, forwardRef,
	ChangeDetectionStrategy, ChangeDetectorRef, ViewChild, Renderer
} from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

const LIST_ITEM_HEIGHT = 42;

export const INPUT_CONTROL_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => InputListComponent),
	multi: true
};

export class InputChange {
	source: InputListComponent;
	value: any[];
}

@Component({
	selector: 'ct-input-list',
	templateUrl: 'input-list.component.html',
	providers: [INPUT_CONTROL_VALUE_ACCESSOR],
	changeDetection: ChangeDetectionStrategy.OnPush,
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	}
})

export class InputListComponent implements ControlValueAccessor {
	@Input('name') name: string;
	@Input('placeholder') placeholder: string = '';
	@Input('type') type: string = 'text';
	@Input('displayName') displayName: string;
	@Input('trackBy') trackBy: string;

	@Input('options')
	set optionsArray(options: any[]) {
		this.options = options;
		this.optionsWithoutFilter = options;
	}

	@Input('maxHeight') maxHeight: number = 168;
	@Input('maxlength') maxlength: number;
	@Input('canClickOverlay') canClickOverlay: boolean = false;

	@Output() change: EventEmitter<InputChange> = new EventEmitter<InputChange>();
	@Output() valueSelected: EventEmitter<any> = new EventEmitter();

	isOpen: boolean = false;
	modelValue: any = '';
	options: any[];
	optionsWithoutFilter: any[];
	private scrollTopNumber: number = 0;
	private selectedObject: any;

	@ViewChild('slimScroll') slimScroll: any;

	private _disabled: boolean = false;
	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private _controlValueAccessorChangeFn: (value: any) => void = (value) => {};
	private onTouched: () => any = () => {};

	constructor(private ref: ChangeDetectorRef,
	            private renderer: Renderer) {
		setTimeout(() => {
			this.slimScroll.scrollContent = this.scrollContent.bind(this);
		}, 0);
	}

	selectOption(option: any, close: boolean = true): void {
		if (option && option.disabled) {
			return;
		}
		this.selectedObject = option;
		this.modelValue = this.getDisplayedName(this.selectedObject);

		if (close) {
			this.isOpen = false;
		}
		this.onTouched();

		this.valueSelected.emit(this.modelValue);
		this._emitChangeEvent();
	}

	getOptionIndex(option: any): number {
		let optionIndex = -1;
		if (option) {
			this.options.forEach((opt, i) => {
				if (this.getOptionValue(opt) === this.getOptionValue(option)) {
					optionIndex = i;
				}
			});
		}
		return optionIndex;
	}

	getOptionValue(option: any): any {
		return this.trackBy ? option[this.trackBy] : option;
	}

	isOptionSelected(option): boolean {
		return this.selectedObject === option;
	}

	getDisplayedName(option: any) {
		return this.displayName ? option[this.displayName] : option;
	}

	updateModel(modelValue: any) {
		this.modelValue = modelValue;
		this.filterOptions();

		this._emitChangeEvent();
	}

	private filterOptions(): void {
		this.options = this.optionsWithoutFilter.filter((option) => this.getDisplayedName(option).indexOf(this.modelValue) > -1);

		if (this.options.length === 0) {
			this.closeList();
		} else {
			this.openList();
		}
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

	_onInputBlur() {
		this.onTouched();
	}

	closeList(): void {
		this.isOpen = false;
		this.ref.markForCheck();
	}

	openList(): void {
		if (this.options.length > 0) {
			this.isOpen = true;
			this.initScrollContent();
		}
	}

	toggleList(): void {
		if (!this._disabled) {
			if (this.isOpen) {
				this.closeList();
			} else {
				this.openList();
			}
		}
	}

	onKeyDown(event: KeyboardEvent): void {
		if (!this.isOpen) {
			return;
		}

		event.stopPropagation();

		let optionIndex = this.getOptionIndex(this.selectedObject);

		if (event.key === 'ArrowDown') {
			optionIndex = optionIndex + 1 < this.options.length ? optionIndex + 1 : optionIndex;
			this.selectedObject = this.options[optionIndex];
			this.modelValue = this.getDisplayedName(this.selectedObject);

			this.changeScrollTop(optionIndex);
			this.slimScroll.scrollContent(this.scrollTopNumber, false, true);
			return;
		}

		if (event.key === 'ArrowUp') {
			optionIndex = optionIndex > 0 ? optionIndex - 1 : 0;
			this.selectedObject = this.options[optionIndex];
			this.modelValue = this.getDisplayedName(this.selectedObject);

			this.changeScrollTop(optionIndex);
			this.slimScroll.scrollContent(this.scrollTopNumber, false, true);
			return;
		}

		if (event.key === 'Enter') {
			event.preventDefault();
			this.toggleList();
		}

		if (event.key === 'Tab') {
			this.closeList();
		}
	}

	private changeScrollTop(optionIndex: number): void {
		if (optionIndex < this.scrollTopNumber) {
			this.scrollTopNumber--;
		} else if (optionIndex > this.scrollTopNumber + 3) {
			this.scrollTopNumber++;
		} else {
			return;
		}
	}

	private initScrollContent(): void {
		setTimeout(() => {
			this.slimScroll.getBarHeight();
			this.scrollTopNumber = this.getOptionIndex(this.selectedObject);
			this.slimScroll.scrollContent(this.scrollTopNumber, false, true);
		}, 0);
	}

	private scrollContent(y: number, isWheel: boolean, isJump: boolean): void {
		let context = this.slimScroll;
		let delta = y;
		let maxTop = context.el.offsetHeight - context.bar.offsetHeight;
		let percentScroll: number;
		let bar = context.bar;
		let el = context.el;

		if (isWheel) {
			if (y === 0) {
				delta = parseInt(getComputedStyle(bar).top, 10);
				percentScroll = delta / (el.offsetHeight - bar.offsetHeight) * (el.scrollHeight - el.offsetHeight);
				el.scrollTop = percentScroll;
			} else {
				el.scrollTop = el.scrollTop + y * LIST_ITEM_HEIGHT;
				delta = el.scrollTop / (el.scrollHeight - el.offsetHeight) * (el.offsetHeight - bar.offsetHeight);
			}
		}

		if (isJump) {
			el.scrollTop = LIST_ITEM_HEIGHT * y;
			delta = y * LIST_ITEM_HEIGHT / (el.scrollHeight - el.offsetHeight) * (el.offsetHeight - bar.offsetHeight);
		}

		delta = Math.min(Math.max(delta, 0), maxTop);
		this.renderer.setElementStyle(bar, 'top', delta + 'px');
	}

	private _emitChangeEvent(): void {
		let event = new InputChange();
		event.source = this;
		event.value = this.modelValue;

		this._controlValueAccessorChangeFn(this.modelValue);
		this.change.emit(event);
	}
}
