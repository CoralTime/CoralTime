import { Client } from './../../models/client';
import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/switchMap';
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
	observable: Observable<any>;
	resolve: any = null;
	subject: Subject<string>;

	@Input('ctClientNameValidator') private client: Client;

	constructor(private clientsService: ClientsService) {
		this.subject = new Subject();
		this.observable = this.subject
			.debounceTime(300)
			.switchMap((clientName: string) => {
				return this.clientsService.getClientByName(clientName);
			}).flatMap((client: Client) => {
				if (client && (!this.client || client.id !== this.client.id)) {
					return Observable.of({ctClientNameInvalid: true});
				} else {
					return Observable.of(null);
				}
			});

		this.observable.subscribe((res) => {
			this.resolvePromise(res);
		})
	}

	resolvePromise(result): void {
		if (this.resolve) {
			this.resolve(result);
			this.resolve = null;
		}
	}

	validate(c: AbstractControl): Promise<{[key: string]: any}> {
		this.resolvePromise(null);

		return new Promise(resolve => {
			this.subject.next(c.value);
			this.resolve = resolve;
		});
	}
}