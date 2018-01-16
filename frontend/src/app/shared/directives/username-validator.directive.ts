import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/switchMap';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';

@Directive({
	selector: '[ctUsernameValidator][formControlName],[ctUsernameValidator][formControl],[ctUsernameValidator][ngModel]',
	providers: [
		{provide: NG_ASYNC_VALIDATORS, useExisting: forwardRef(() => UsernameValidator), multi: true}
	]
})

export class UsernameValidator implements Validator {
	observable: Observable<any>;
	subject: Subject<string>;
	resolve: any = null;

	@Input('ctUsernameValidator') private user: User;

	constructor(private userService: UsersService) {
		this.subject = new Subject();
		this.observable = this.subject
			.debounceTime(300)
			.switchMap((username: string) => {
				return this.userService.getUserByUsername(username);
			}).flatMap(user => {
				if (user && (!this.user || user.id !== this.user.id)) {
					return Observable.of({ctUsernameInvalid: true});
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
