import {
	Component, Input, Output, EventEmitter, forwardRef, ViewChild, OnChanges, SimpleChanges, HostListener
} from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { SelectItem } from 'primeng/components/common/api';
import { MultiSelect } from 'primeng/components/multiselect/multiselect';
import { ObjectUtils } from 'primeng/components/utils/objectutils';
import { DomHandler } from 'primeng/primeng';

export class CustomSelectItem implements SelectItem {
	isActive?: boolean;
	label: string;
	value: any;

	constructor(label: string, value: any, isActive?: boolean) {
		this.label = label;
		this.value = value;
		this.isActive = isActive != null ? isActive : true;
	}
}

export const MULTISELECT_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => MultiSelectComponent),
	multi: true
};

@Component({
	selector: 'ct-multiselect',
	templateUrl: 'multiselect.component.html',
	providers: [DomHandler, ObjectUtils, MULTISELECT_VALUE_ACCESSOR]
})

export class MultiSelectComponent extends MultiSelect implements OnChanges {
	@Input() ngModel: number[];
	@Input() extraActionTitle: string;
	@Input() showSubmitButton: boolean = false;
	@Input() showFilterSearch: boolean = true;
	@Input() showActionsPanel: boolean = true;

	@Output() onExtraAction: EventEmitter<any> = new EventEmitter();
	@Output() onSubmitAction: EventEmitter<any> = new EventEmitter();

	@ViewChild('slimScroll') slimScroll: any;

	clickListener: any;
	isSubmitted: boolean = false;
	filter: boolean;
	oldValue: any[];

	show(): void {
		super.show();
		this.redrowSlimScroll();

		if (this.showSubmitButton) {
			this.cashValue();
		}
	}

	hide(): void {
		this.isSubmitted = false;
		super.hide();
	}

	ngOnChanges(changes: SimpleChanges) {
		if (changes && changes['ngModel'] && changes['ngModel'].currentValue.length === 0) {
			setTimeout(() => {
				this.valuesAsString = this.defaultLabel;
			}, 0);
		}
	}

	clearFilter(): void {
		this.filterValue = null;
		this.redrowSlimScroll();
	}

	onFilter(event): void {
		super.onFilter(event);
		this.redrowSlimScroll();
	}

	selectAll(): void {
		let opts = this.getVisibleOptions();
		if (opts) {
			this.value = [];
			for (let i = 0; i < opts.length; i++) {
				this.value.push(opts[i].value);
			}
		}
		this.onModelChange(this.value);
		this.onChange.emit({originalEvent: event, value: this.value});
	}

	selectNone(): void {
		this.value = [];
		this.onModelChange(this.value);
		this.onChange.emit({originalEvent: event, value: this.value});
	}

	doExtraAction(event): void {
		this.onExtraAction.emit(event);
	}

	submit($event): void {
		this.isSubmitted = true;
		this.oldValue = this.value;
		this.onSubmitAction.emit($event);
		this.close($event);
	}

	toString(value: string): string {
		return value !== null + '' ? value : this.defaultLabel.slice(4) + ' (1)';
	}

	private cashValue(): void {
		this.oldValue = this.value;
		if (!this.clickListener) {
			this.clickListener = this.renderer.listen('document', 'click', () => {
				if (!this.overlayVisible && !this.isSubmitted) {
					this.clearCash();
					this.value = this.oldValue;
				}
			});
		}
	}

	private clearCash(): void {
		this.clickListener();
		this.clickListener = null;
	}

	private redrowSlimScroll(): void {
		setTimeout(() => {
			if (this.slimScroll) {
				this.slimScroll.getBarHeight();
			}
		}, 0);
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (this.overlayVisible && event.key === 'Enter') {
			this.submit(event);
		}
	}
}
