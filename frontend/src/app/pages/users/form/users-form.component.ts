import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable } from 'rxjs/Observable';

import { User } from '../../../models/user';
import { Roles } from '../../../core/auth/permissions';
import { AuthService } from '../../../core/auth/auth.service';
import { AuthUser } from '../../../core/auth/auth-user';
import { ArrayUtils } from '../../../core/object-utils';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { UsersService } from '../../../services/users.service';

class FormUser {
	email: string;
	fullName: string;
	id: number;
	isActive: boolean;
	role: number;
	userName: string;

	static fromUser(user: User) {
		let instance = new this;
		instance.id = user.id;
		instance.fullName = user.fullName;
		instance.userName = user.userName;
		instance.email = user.email;
		instance.isActive = user.id ? user.isActive : true;

		if (user.isAdmin) {
			instance.role = Roles.admin;
		} else if (user.id) {
			instance.role = Roles.user;
		}

		return instance;
	}

	toUser(user: User): User {
		return new User({
			dateFormat: user.dateFormat,
			dateFormatId: user.dateFormatId,
			defaultProjectId: user.defaultProjectId,
			defaultTaskId: user.defaultTaskId,
			email: this.email,
			fullName: this.fullName,
			id: this.id,
			isActive: this.isActive,
			isAdmin: this.role === Roles.admin,
			isManager: user.isManager,
			isWeeklyTimeEntryUpdatesSend: user.isWeeklyTimeEntryUpdatesSend,
			projectsCount: user.projectsCount,
			sendEmailDays: user.sendEmailDays,
			sendEmailTime: user.sendEmailTime,
			timeFormat: user.timeFormat,
			userName: this.userName,
			weekStart: user.weekStart
		});
	}
}

@Component({
	selector: 'ct-user-form',
	templateUrl: 'users-form.component.html',
	providers: [TranslatePipe]
})

export class UsersFormComponent implements OnInit {
	@Input() user: User;
	@Output() onSaved = new EventEmitter();

	authUser: AuthUser;
	submitButtonText: string;
	dialogHeader: string;
	emailPattern = EMAIL_PATTERN;
	isRequestLoading: boolean = false;
	impersonateUser: User;
	isActive: boolean;
	isNewUser: boolean;
	model: FormUser;
	roleModel: any;
	showErrors: boolean[] = []; // [showEmailError, showFullNameError, showUserNameError]
	stateModel: any;
	stateText: string;
	userNotification: string;

	roles = [
		{value: Roles.admin, title: 'admin'},
		{value: Roles.user, title: 'user'}
	];

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'deactivated'}
	];

	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private translatePipe: TranslatePipe,
	            private userService: UsersService) { }

	ngOnInit() {
		this.authUser = this.authService.authUser;
		this.impersonateUser = this.impersonationService.impersonationUser;

		let user = this.user;
		this.isNewUser = !user;
		this.user = user ? user : new User();
		this.submitButtonText = this.user.id ? 'Save' : 'Create';

		this.model = FormUser.fromUser(this.user);
		this.roleModel = this.user.id ? this.roles.filter((role) => role.value === this.model.role)[0] : this.roles[1];
		this.dialogHeader = this.user.id ? 'Edit' : this.translatePipe.transform('Create New User');
		this.userNotification = this.user.id ? 'Send update account email' : 'Send invitation email';
		this.stateModel = ArrayUtils.findByProperty(this.states, 'value', this.model.isActive);
		this.stateText = this.user.isActive ? '' : 'Time entries of the deactivated user are still editable for managers.';
	}

	stateOnChange(): void {
		this.model.isActive = this.stateModel.value;
		this.stateText = this.stateModel.value ? '' : 'Time entries of the deactivated user are still editable for managers.';
	}

	roleOnChange(): void {
		this.model.role = this.roleModel.value;
	}

	validateAndSubmit(form: NgForm): void {
		this.isRequestLoading = true;
		this.validateForm(form)
			.subscribe((isFormValid: boolean) => {
					this.isRequestLoading = false;
					if (isFormValid) {
						this.submit();
					}
				},
				() => this.isRequestLoading = false);
	}

	private submit(): void {
		let submitObservable: Observable<any>;
		let updatedUser = this.model.toUser(this.user);

		if (updatedUser.id) {
			submitObservable = this.userService.odata.Put(updatedUser, updatedUser.id.toString());
		} else {
			submitObservable = this.userService.odata.Post(updatedUser);
		}

		this.isRequestLoading = true;
		submitObservable.toPromise().then(
			() => {
				this.isRequestLoading = false;

				if (this.impersonationService.impersonationId && this.impersonationService.impersonationUser.id === updatedUser.id) {
					this.impersonationService.impersonationUser = updatedUser;
					this.impersonationService.setStorage(updatedUser);
					this.impersonationService.onChange.emit(updatedUser);
				}

				if (this.authUser.id === updatedUser.id) {
					this.userService.setUserInfo(updatedUser);
				}

				this.onSaved.emit({
					isNewUser: this.isNewUser
				});
			},
			error => this.onSaved.emit({
				isNewUser: this.isNewUser,
				error: error
			}));
	}

	private validateForm(form: NgForm): Observable<boolean> {
		this.showErrors = [false, false, false];

		let isEmailValidObservable: Observable<any>;
		let isUserNameValidObservable: Observable<any>;

		if (!this.model.email || !!form.controls['email'].errors) {
			isEmailValidObservable = Observable.of(false);
		} else {
			isEmailValidObservable = this.userService.getUserByEmail(this.model.email)
				.map((user) => !user || (user.id === this.model.id));
		}

		if (!this.model.userName) {
			isUserNameValidObservable = Observable.of(false);
		} else {
			isUserNameValidObservable = this.userService.getUserByUsername(this.model.userName)
				.map((user) => !user || (user.id === this.model.id));
		}

		return Observable.forkJoin(isEmailValidObservable, Observable.of(!!this.model.fullName), isUserNameValidObservable)
			.map((response: boolean[]) =>
				response.map((isControlValid, i) => this.showErrors[i] = !isControlValid)
					.every((showError) => showError === false)
			);
	}
}
