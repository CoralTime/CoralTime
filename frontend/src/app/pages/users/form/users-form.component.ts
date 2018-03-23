import { Observable } from 'rxjs/Observable';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { UsersService } from '../../../services/users.service';
import { User } from '../../../models/user';
import { Roles } from '../../../core/auth/permissions';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { AuthUser } from '../../../core/auth/auth-user';
import { ArrayUtils } from '../../../core/object-utils';
import { NgForm } from '@angular/forms';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import { ImpersonationService } from '../../../services/impersonation.service';

class FormUser {
	confirmPassword: string;
	email: string;
	fullName: string;
	id: number;
	isActive: boolean;
	password: string;
	role: number;
	sendInvitationEmail: boolean;
	userName: string;

	static fromUser(user: User) {
		let instance = new this;
		instance.id = user.id;
		instance.fullName = user.fullName;
		instance.userName = user.userName;
		instance.password = user.password;
		instance.email = user.email;
		instance.sendInvitationEmail = user.sendInvitationEmail;
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
			password: this.password,
			projectsCount: user.projectsCount,
			sendEmailDays: user.sendEmailDays,
			sendEmailTime: user.sendEmailTime,
			sendInvitationEmail: this.sendInvitationEmail || false,
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

	dialogHeader: string;
	emailPattern = EMAIL_PATTERN;
	isRequestLoading: boolean = false;
	model: FormUser;
	roleModel: any;
	submitButtonText: string;
	userNotification: string;

	errorFullNameMessage: boolean;
	errorUserNameMessage: boolean;
	errorEmailMessage: boolean;
	errorPasswordMessage: boolean;
	errorConfirmPasswordMessage: boolean;
	isEmailValid: boolean = true;

	authUser: AuthUser;
	impersonateUser: User;

	roles = [
		{value: Roles.admin, title: 'admin'},
		{value: Roles.user, title: 'user'}
	];

	stateModel: any;
	isActive: boolean;
	isNewUser: boolean;
	stateText: string;

	states = [
		{value: true, title: 'active'},
		{value: false, title: 'deactivated'}
	];

	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private translatePipe: TranslatePipe,
	            private userService: UsersService) { }

	ngOnInit() {
		this.authUser = this.authService.getAuthUser();
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

	userFormValidate(): void {
		if (!this.model.fullName) {
			this.errorFullNameMessage = true;
		}

		if (!this.model.userName) {
			this.errorUserNameMessage = true;
		}

		if (!this.model.email) {
			this.errorEmailMessage = true;
		}

		if (this.model.email) {
			this.isEmailValid = this.validateEmail(this.model.email);
		}

		if (!this.model.password) {
			this.errorPasswordMessage = true;
		}

		if (!this.model.confirmPassword) {
			this.errorConfirmPasswordMessage = true;
		}
	}

	validateEmail(email): boolean {
		return EMAIL_PATTERN.test(email);
	}

	save(userForm: NgForm): void {
		this.userFormValidate();

		if (!userForm.valid) {
			return;
		}

		let updatedUser = this.model.toUser(this.user);
		let submitObservable: Observable<any>;

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
}
