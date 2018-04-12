import {
	Component, Input, Output, EventEmitter, forwardRef, ChangeDetectorRef, ChangeDetectionStrategy, ViewChild, Renderer,
	ElementRef
} from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

const LIST_ITEM_HEIGHT = 42;

export const SELECT_CONTROL_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => SelectComponent),
	multi: true
};

export class SelectChange {
	source: SelectComponent;
	value: any[];
}

@Component({
	selector: 'ct-select',
	templateUrl: 'select.component.html',
	providers: [SELECT_CONTROL_VALUE_ACCESSOR],
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	},
	changeDetection: ChangeDetectionStrategy.OnPush  // Fixed in newest version https://github.com/angular/material2/pull/2894
})

export class SelectComponent implements ControlValueAccessor {
	@Input('name') name: string;
	@Input('displayName') displayName: string;
	@Input('trackBy') trackBy: string;
	@Input('options') options: any[];
	@Input('defaultValue') defaultValue: string;
	@Input('icon') icon: string;
	@Input('canClickOverlay') canClickOverlay: boolean = false;
	@Input('maxHeight') maxHeight: number = 168;
	@Input('container') container: HTMLDivElement;
	@Output() change: EventEmitter<SelectChange> = new EventEmitter<SelectChange>();

	isOpen: boolean = false;
	isAnimate: boolean = false;
	isListShowToTop: boolean = false;
	selectedObject: any;

	@ViewChild('slimScroll') slimScroll: any;
	@ViewChild('matList', {read: ElementRef}) matList: ElementRef;

	private _disabled: boolean = false;
	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private oldSelectedObject: any;
	private scrollTopNumber: number = 0;

	private _controlValueAccessorChangeFn: (value: any) => void = () => {};
	private onTouched: () => any = () => {};

	constructor(private el: ElementRef,
	            private ref: ChangeDetectorRef,
	            private renderer: Renderer) {
		setTimeout(() => {
			this.slimScroll.scrollContent = this.scrollContent.bind(this);
		}, 0);
	}

	getSelectedOptionsText() {
		return this.selectedObject ? this.getDisplayedName(this.selectedObject) : this.defaultValue;
	}

	selectOption(option: any, close: boolean = true) {
		if (option && option.disabled) {
			return;
		}
		this.selectedObject = option;

		if (close) {
			this.isOpen = false;
		}
		this.onTouched();

		if (this.getOptionValue(option) !== this.getOptionValue(this.oldSelectedObject)) {
			this._emitChangeEvent();
		}
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
		return option ? (this.trackBy ? option[this.trackBy] : option) : null;
	}

	isOptionSelected(option): boolean {
		return this.selectedObject === option;
	}

	getDisplayedName(option: any) {
		return option ? (this.displayName ? option[this.displayName] : option) : '';
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(selectedObject: any = null) {
		this.selectedObject = selectedObject;
		this.ref.markForCheck();
		if (this.selectedObject) {
			this._controlValueAccessorChangeFn(this.selectedObject);
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

	closeSelect(): void {
		this.isOpen = false;
		this.isAnimate = false;
	}

	openSelect(): void {
		this.isOpen = true;
		this.oldSelectedObject = this.selectedObject;
		setTimeout(() => {
			this.canShowList();
			this.isAnimate = true;
			this.ref.markForCheck();
		}, 0);
	}

	toggleSelect(): void {
		if (!this._disabled) {
			if (this.isOpen) {
				this.closeSelect();
			} else {
				this.openSelect();
				this.initScrollContent();
			}
		}
	}

	onKeyDown(event: KeyboardEvent): void {
		if (!this.isOpen) {
			return;
		}

		event.preventDefault();
		event.stopPropagation();

		let optionIndex = this.getOptionIndex(this.selectedObject);

		if (event.key === 'ArrowDown') {
			optionIndex = optionIndex + 1 < this.options.length ? optionIndex + 1 : optionIndex;
			this.selectedObject = this.options[optionIndex];

			this.changeScrollTop(optionIndex);
			this.slimScroll.scrollContent(this.scrollTopNumber, false, true);
			return;
		}

		if (event.key === 'ArrowUp') {
			optionIndex = optionIndex > 0 ? optionIndex - 1 : 0;
			this.selectedObject = this.options[optionIndex];

			this.changeScrollTop(optionIndex);
			this.slimScroll.scrollContent(this.scrollTopNumber, false, true);
			return;
		}

		if (event.key === 'Enter') {
			this.selectOption(this.selectedObject);
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

	private _emitChangeEvent() {
		let event = new SelectChange();
		event.source = this;
		event.value = this.selectedObject;

		this._controlValueAccessorChangeFn(this.selectedObject);
		this.change.emit(event);
	}

	/**
	 * Display option list to top
	 */

	private canShowList(): void {
		let listHeight = this.matList.nativeElement.offsetHeight;

		if (this.container) {
			this.isListShowToTop = !this.isBottomClear(listHeight) && this.isTopClear(listHeight);
		}
	}

	private isBottomClear(listHeight: number): boolean {
		let elBottom: number = this.el.nativeElement.getBoundingClientRect().bottom;
		let containerBottom: number = this.container.getBoundingClientRect().bottom;

		return containerBottom > elBottom + listHeight + 5;
	}

	private isTopClear(listHeight: number): boolean {
		let elTop: number = this.el.nativeElement.getBoundingClientRect().top;
		let containerTop: number = this.container.getBoundingClientRect().top;

		return listHeight + containerTop + 40 < elTop;
	}
}
