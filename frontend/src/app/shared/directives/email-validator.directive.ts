import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/debounceTime';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';

@Directive({
	selector: '[ctEmailValidator][formControlEmail],[ctEmailValidator][formControl],[ctEmailValidator][ngModel]',
	providers: [
		{provide: NG_ASYNC_VALIDATORS, useExisting: forwardRef(() => EmailValidator), multi: true}
	]
})

export class EmailValidator implements Validator {
	observable: Observable<any>;
	subject: Subject<string>;
	resolve: any = null;

	@Input('ctEmailValidator') private user: User;

	constructor(private userService: UsersService) {
		this.subject = new Subject();
		this.observable = this.subject
			.debounceTime(300)
			.switchMap((email: string) => {
				return this.userService.getUserByEmail(email);
			}).flatMap(user => {
				if (user && (!this.user || user.id !== this.user.id)) {
					return Observable.of({ctEmailInvalid: true});
				} else {
					return Observable.of(null);
				}
			});

		this.observable.subscribe((res) => {
			this.resolvePromise(res);
		});
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
