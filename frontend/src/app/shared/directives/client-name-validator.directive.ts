import { Client } from '../../models/client';
import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { ClientsService } from '../../services/clients.service';

@Directive({
	selector: '[ctClientNameValidator][formControlName],[ctClientNameValidator][formControl],[ctClientNameValidator][ngModel]',
	providers: [
		{
			provide: NG_ASYNC_VALIDATORS,
			useExisting: forwardRef(() => ClientNameValidator),
			multi: true
		}
	]
})

export class ClientNameValidator implements Validator {
	@Input('ctClientNameValidator') client: Client;

	constructor(private clientsService: ClientsService) {
	}

	validate(control: AbstractControl): Observable<{ [key: string]: any }> {
		return control.valueChanges
			.debounceTime(500)
			.take(1)
			.switchMap(() => {
				return this.clientsService.getClientByName(control.value);
			})
			.map(client => {
				if (client && (!this.client || client.id !== this.client.id)) {
					return {ctClientNameInvalid: true};
				}

				return null;
			})
			.first()
	}
}
