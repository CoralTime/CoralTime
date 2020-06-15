import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Component, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { Client } from '../../../../models/client';
import { Subscription } from 'rxjs/Subscription';

@Component({
	selector: 'ct-reports-send-form',
	templateUrl: 'reports-send-form.component.html'
})

export class ReportsSendFormComponent implements OnDestroy {
	@Input() buttonText: string;
	@Input() clients: Client[];
	@Input() emailValues: string[] = [];
	@Input() isFormErrorsShown: boolean;

	@Output() formChanged = new EventEmitter();

	form: FormGroup;
	private subscription: Subscription;

	constructor(private fb: FormBuilder) {
		this.form = this.fb.group({
			emails: new FormArray([
				new FormControl('', Validators.required)
			])
		});
		this.subscription = this.form.valueChanges.subscribe(() => {
			this.formChanged.emit(this.form.valid);
		});
	}

	addNewEmail(): void {
		const arrayControl = <FormArray>this.form.controls['emails'];
		let newControl = new FormControl('', Validators.required);
		arrayControl.push(newControl);
	}

	delEmail(index: number): void {
		const arrayControl = <FormArray>this.form.controls['emails'];
		arrayControl.removeAt(index);
		this.emailValues.splice(index, 1);
	}

	get emails(): FormArray { return this.form.get('emails') as FormArray; }

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}
}
