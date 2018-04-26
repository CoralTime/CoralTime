import {
	Component, Input, Output, EventEmitter, forwardRef, ViewChild, HostListener
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

export class MultiSelectComponent extends MultiSelect {
	@Input() extraActionTitle: string;
	@Input() showSubmitButton: boolean = false;
	@Input() showFilterSearch: boolean = true;
	@Input() showActionsPanel: boolean = true;

	@Output() onExtraAction: EventEmitter<any> = new EventEmitter();
	@Output() onSubmitAction: EventEmitter<any> = new EventEmitter();

	@ViewChild('slimScroll') slimScroll: any;

	isSubmitted: boolean = false;
	oldValue: any[];

	show(): void {
		super.show();
		this.redrowSlimScroll();

		if (this.showSubmitButton) {
			this.isSubmitted = false;
			this.oldValue = this.value;
		}
	}

	hide(): void {
		super.hide();
		this.clearFilter();

		if (this.showSubmitButton && !this.isSubmitted) {
			this.value = this.oldValue;
			this.updateLabel();
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

	selectAll(event: MouseEvent): void {
		super.toggleAll(event, {
			checked: false
		});
	}

	selectNone(event: MouseEvent): void {
		super.toggleAll(event, {
			checked: true
		});
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

	private redrowSlimScroll(): void {
		setTimeout(() => {
			this.slimScroll.getBarHeight();
		}, 0);
	}

	@HostListener('document:keydown', ['$event'])
	onKeyDown(event: KeyboardEvent) {
		if (this.overlayVisible && event.key === 'Enter') {
			this.submit(event);
		}
	}
}
