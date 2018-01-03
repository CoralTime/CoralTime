import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import {
	Component, OnInit, Input, Self, Optional, ContentChildren, QueryList, ChangeDetectorRef, Output, EventEmitter, AfterContentInit
} from '@angular/core';
import { coerceBooleanProperty, MdOption, SelectionModel } from '@angular/material';

export class MdColorPickerChange {
	constructor(public source: ColorPickerComponent,
	            public value: any) {
	}
}

@Component({
	selector: 'ct-color-picker',
	templateUrl: 'color-picker.component.html',
	exportAs: 'ctColorPicker',
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	}
})

export class ColorPickerComponent implements OnInit, AfterContentInit, ControlValueAccessor {
	private _panelOpen: boolean = false;
	private _disabled: boolean = false;
	/** Subscription to changes in the option list. */
	private _changeSubscription: Subscription;
	/** Subscriptions to option events. */
	private _optionSubscription: Subscription;

	_onChange: (value: any) => void = () => { };
	_onTouched = () => { };

	@Input()
	get disabled() { return this._disabled; }

	set disabled(value: any) {
		this._disabled = coerceBooleanProperty(value);
	}

	@Output() change: EventEmitter<MdColorPickerChange> = new EventEmitter<MdColorPickerChange>();

	/** Deals with the selection logic. */
	_selectionModel: SelectionModel<MdOption>;

	/** Combined stream of all of the child options' change events. */
	get optionSelectionChanges(): Observable<any> {
		return Observable.merge(...this.options.map(option => option.onSelectionChange));
	}

	/** The currently selected option. */
	get selected(): MdOption | MdOption[] {
		return this._selectionModel.selected[0];
	}

	/** The value displayed in the trigger. */
	get triggerValue(): string {
		return this._selectionModel.selected[0].value;
	}

	@ContentChildren(MdOption, {descendants: true}) options: QueryList<MdOption>;

	constructor(private _changeDetectorRef: ChangeDetectorRef,
	            @Self() @Optional() public _control: NgControl,) {
		if (this._control) {
			this._control.valueAccessor = this;
		}
	}

	ngOnInit() {
		this._selectionModel = new SelectionModel<MdOption>(false, null, false);
	}

	ngAfterContentInit() {
		this._changeSubscription = this.options.changes.startWith(null).subscribe(() => {
			this._resetOptions();

			if (this._control) {
				// Defer setting the value in order to avoid the "Expression
				// has changed after it was checked" errors from Angular.
				Promise.resolve(null).then(() => this._setSelectionByValue(this._control.value));
			}
		});
	}

	onKeyDown(event){
		if (!this._panelOpen) {
			return;
		}

		event.preventDefault();
		event.stopPropagation();

		let maxValue = this.options.length - 1;
		let halfOfMaxValue = Math.ceil(maxValue / 2);

		if (event.key == 'ArrowDown') {
			let selectedValue = +this.triggerValue < halfOfMaxValue ?  +this.triggerValue + halfOfMaxValue : this.triggerValue;
			this._setSelectionByValue(selectedValue);
			return;
		}

		if (event.key == 'ArrowUp') {
			let selectedValue = +this.triggerValue >= halfOfMaxValue ?  +this.triggerValue - halfOfMaxValue : this.triggerValue;
			this._setSelectionByValue(selectedValue);
			return;
		}

		if (event.key == 'ArrowLeft') {
			let selectedValue = Math.max(+this.triggerValue - 1, 0);
			this._setSelectionByValue(selectedValue);
			return;
		}

		if (event.key == 'ArrowRight') {
			let selectedValue = Math.min(+this.triggerValue + 1, maxValue);
			this._setSelectionByValue(selectedValue);
			return;
		}

		if (event.key === 'Enter') {
			this.toggle();
			return;
		}
	}

	toggle(): void {
		this.panelOpen ? this.close() : this.open();
	}

	/** Opens the overlay panel. */
	open(): void {
		if (this.disabled) {
			return;
		}
		this._panelOpen = true;
	}

	/** Closes the overlay panel and focuses the host element. */
	close(): void {
		if (this._panelOpen) {
			this._panelOpen = false;

		}
	}

	get panelOpen(): boolean {
		return this._panelOpen;
	}

	writeValue(value: any): void {
		if (this.options) {
			this._setSelectionByValue(value);
		}
	}

	registerOnChange(fn: (value: any) => void): void {
		this._onChange = fn;
	}

	registerOnTouched(fn: () => {}): void {
		this._onTouched = fn;
	}

	setDisabledState(isDisabled: boolean): void {
		this.disabled = isDisabled;
	}

	private _resetOptions(): void {
		this._dropSubscriptions();
		this._listenToOptions();
	}

	private _dropSubscriptions(): void {
		if (this._optionSubscription) {
			this._optionSubscription.unsubscribe();
			this._optionSubscription = null;
		}
	}

	private _listenToOptions(): void {
		this._optionSubscription = this.optionSelectionChanges
			.filter(event => event.isUserInput)
			.subscribe(event => {
				this._onSelect(event.source);

				this.close();
			});
	}

	private _setSelectionByValue(value: any | any[], isUserInput = false): void {
		const isArray = Array.isArray(value);

		this._clearSelection();

		if (isArray) {
			value.forEach((currentValue: any) => this._selectValue(currentValue, isUserInput));
		} else {
			this._selectValue(value, isUserInput);
		}

		this._changeDetectorRef.markForCheck();
	}

	/**
	 * Finds and selects and option based on its value.
	 * @returns Option that has the corresponding value.
	 */
	private _selectValue(value: any, isUserInput = false): MdOption {
		let optionsArray = this.options.toArray();
		let correspondingOption = optionsArray.find(option => {
			return option.value != null && option.value === value;
		});

		if (correspondingOption) {
			isUserInput ? correspondingOption._selectViaInteraction() : correspondingOption.select();
			this._selectionModel.select(correspondingOption);
		}

		return correspondingOption;
	}

	/**
	 * Clears the select trigger and deselects every option in the list.
	 * @param skip Option that should not be deselected.
	 */
	private _clearSelection(skip?: MdOption): void {
		this._selectionModel.clear();
		this.options.forEach(option => {
			if (option !== skip) {
				option.deselect();
			}
		});
	}

	/** Invoked when an option is clicked. */
	private _onSelect(option: MdOption): void {
		const wasSelected = this._selectionModel.isSelected(option);

		this._clearSelection(option.value == null ? null : option);

		if (option.value == null) {
			this._propagateChanges(option.value);
		} else {
			this._selectionModel.select(option);
		}

		if (wasSelected !== this._selectionModel.isSelected(option)) {
			this._propagateChanges();
		}
	}

	/** Emits change event to set the model value. */
	private _propagateChanges(fallbackValue?: any): void {
		let valueToEmit = null;

		if (Array.isArray(this.selected)) {
			valueToEmit = this.selected.map(option => option.value);
		} else {
			valueToEmit = this.selected ? this.selected.value : fallbackValue;
		}

		this._onChange(valueToEmit);
		this.change.emit(new MdColorPickerChange(this, valueToEmit));
	}
}