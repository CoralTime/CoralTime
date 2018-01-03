import { Component, Output, EventEmitter } from '@angular/core';

@Component({
	selector: 'ct-confirmation-dialog',
	templateUrl: 'confirmation.component.html'
})

export class ConfirmationComponent {
	@Output() onSubmit = new EventEmitter();

	submit(confirm: boolean) {
		this.onSubmit.emit(confirm)
	}
}