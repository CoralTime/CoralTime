import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';

@Directive({
	selector: '[ctEmailValidator][formControlEmail],[ctEmailValidator][formControl],[ctEmailValidator][ngModel]',
	providers: [
		{provide: NG_ASYNC_VALIDATORS, useExisting: forwardRef(() => EmailValidator), multi: true}
	]
})

export class EmailValidator implements Validator {
	@Input('ctEmailValidator') user: User;

	constructor(private userService: UsersService) {
	}

	validate(control: AbstractControl): Observable<{ [key: string]: any }> {
		return control.valueChanges
			.debounceTime(500)
			.take(1)
			.switchMap(() => {
				return this.userService.getUserByEmail(control.value);
			})
			.map(user => {
				if (user && (!this.user || user.id !== this.user.id)) {
					return {ctEmailInvalid: true};
				}

				return null;
			})
			.first()
	}
}
