import { Directive } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
	selector: '[ctValidateOnblur]',
	host: {
		'(focus)': 'onFocus($event)',
		'(blur)': 'onBlur($event)'
	}
})

export class ValidationOnBlurDirective {
	private validators: any;
	private asyncValidators: any;
	private hasFocus = false;

	constructor(public formControl: NgControl) {
	}

	onFocus($event) {
		this.hasFocus = true;
		this.validators = this.formControl.control.validator;
		this.asyncValidators = this.formControl.control.asyncValidator;
		this.formControl.control.clearAsyncValidators();
		this.formControl.control.clearValidators();
		this.formControl.control.valueChanges
			.filter(() => this.hasFocus)
			.subscribe(() => this.formControl.control.markAsPending());
	}

	onBlur($event) {
		this.hasFocus = false;
		this.formControl.control.setAsyncValidators(this.asyncValidators);
		this.formControl.control.setValidators(this.validators);
		this.formControl.control.updateValueAndValidity();
	}
}
