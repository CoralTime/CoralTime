import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { ArrayUtils } from '../../../core/object-utils';
import { User } from '../../../models/user';
import { Project } from '../../../models/project';
import { Task } from '../../../models/task';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import { NotificationService } from '../../../core/notification.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { ProjectsService } from '../../../services/projects.service';
import { TasksService } from '../../../services/tasks.service';
import { DateFormat, NOT_FULL_WEEK_DAYS, ProfileService, TimeFormat, WeekDay } from '../../../services/profile.service';
import { EnterEmailService } from '../../set-password/enter-email/enter-email.service';
import { UserPicService } from '../../../services/user-pic.service';
import { UsersService } from '../../../services/users.service';
import { ProfilePhotoComponent } from './profile-photo/profile-photo.component';

const STANDART_TIME_ARRAY = [
	'0:00', '1:00', '2:00', '3:00', '4:00', '5:00',
	'6:00', '7:00', '8:00', '9:00', '10:00', '11:00', '12:00',
	'13:00', '14:00', '15:00', '16:00', '17:00', '18:00',
	'19:00', '20:00', '21:00', '22:00', '23:00'
];
const TWELVE_CLOCK_TIME_ARRAY = [
	'12:00 AM', '1:00 AM', '2:00 AM', '3:00 AM', '4:00 AM', '5:00 AM',
	'6:00 AM', '7:00 AM', '8:00 AM', '9:00 AM', '10:00 AM', '11:00 AM',
	'12:00 PM', '1:00 PM', '2:00 PM', '3:00 PM', '4:00 PM', '5:00 PM',
	'6:00 PM', '7:00 PM', '8:00 PM', '9:00 PM', '10:00 PM', '11:00 PM'
];

@Component({
	selector: 'ct-profile-settings',
	templateUrl: 'profile-settings.component.html'
})

export class ProfileSettingsComponent implements OnInit {
	user: User;
	userModel: User = new User();

	avatarUrl: string;
	dateFormats: DateFormat[];
	dateFormatModel: DateFormat;
	emailPattern = EMAIL_PATTERN;
	isEmailChanged: boolean;
	preferencesFormStatus = {};
	personalInfoFormStatus = {};
	isTasksLoading: boolean;
	numberMask = [/\d/, /\d/];
	projects: Project[];
	projectModel: Project;
	resetPasswordMessage: string;
	sendEmailDaysArray: WeekDay[];
	sendEmailDays: boolean[] = [];
	sendEmailTimeModel: string;
	sendEmailTimeArray: string[];
	showWrongEmailMessage: boolean;
	tasks: Task[];
	taskModel: Task;
	timeFormats: TimeFormat[] = [new TimeFormat(12), new TimeFormat(24)];
	timeFormatModel: TimeFormat = this.timeFormats[1];
	weekStartDays: string[] = ['Sunday', 'Monday'];
	weekStartDayModel: string;

	private dialogRef: MatDialogRef<ProfilePhotoComponent>;

	@ViewChild('personalInfoForm') personalInfoForm: NgForm;
	@ViewChild('preferencesForm') preferencesForm: NgForm;

	constructor(private dialog: MatDialog,
	            private enterEmailService: EnterEmailService,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private projectsService: ProjectsService,
	            private profileService: ProfileService,
	            private route: ActivatedRoute,
	            private tasksService: TasksService,
	            private userPicService: UserPicService,
	            private usersService: UsersService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			this.user = this.impersonationService.impersonationUser || data.user;
			this.userModel = new User(this.user);
		});

		this.avatarUrl = this.userModel.urlIcon && this.userModel.urlIcon.replace('Icons', 'Avatars');
		this.timeFormatModel = this.userModel.timeFormat ? new TimeFormat(this.userModel.timeFormat) : this.timeFormats[1];
		this.weekStartDayModel = this.weekStartDays[this.userModel.weekStart];

		this.getAvatar();
		this.getDateFormats();
		this.getProjects();
		this.setSendEmailTimeData(this.userModel.timeFormat);
		this.setSendEmailWeekDaysArray(this.userModel.weekStart);
	}

	getAvatar(): Promise<string> {
		return this.userPicService.loadUserPicture(this.userModel.id)
			.toPromise()
			.then((avatarUrl: string) => this.avatarUrl = avatarUrl);
	}

	isGravatarIcon(avatarUrl: string): boolean {
		return avatarUrl.includes('gravatar');
	}

	openPhotoDialog(): void {
		this.dialogRef = this.dialog.open(ProfilePhotoComponent);

		this.dialogRef.componentInstance.onSubmit.subscribe((avatarUrl: string) => {
			this.dialogRef.close();
			this.updateAvatar(avatarUrl);
		});
	}

	resetPassword(): void {
		this.enterEmailService.sendEmail(this.userModel.email).subscribe(
			(emailResponse) => {
				if (emailResponse.isSentEmail) {
					this.resetPasswordMessage = 'Email to reset password sent to ' + this.userModel.email;
					setTimeout(() => this.resetPasswordMessage = '', 5000);
				} else {
					this.notificationService.danger('Error when resetting the password.');
				}
			},
			() => this.notificationService.danger('Error when resetting the password.'));
	}

	updateAvatar(avatarUrl: string): void {
		this.avatarUrl = avatarUrl;
		const iconObject = {
			urlIcon: avatarUrl.replace('Avatars', 'Icons').replace('s=200', 's=40')
		};

		if (this.impersonationService.impersonationId) {
			let impersonateUser = Object.assign(this.impersonationService.impersonationUser, iconObject);
			this.impersonationService.setStorage(impersonateUser);
		} else {
			this.usersService.setUserInfo(iconObject);
		}
	}

	// FORM CHANGED

	fullNameOnChange(): void {
		this.submitPersonalInfo('fullName');
	}

	emailOnBlur(): void {
		this.submitPersonalInfo('email');
	}

	emailOnChange(): void {
		this.isEmailChanged = true;
		this.showWrongEmailMessage = false;
	}

	defaultProjectOnChange(): void {
		this.loadTasks(this.projectModel.id);
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['defaultProjectId'] = this.projectModel.id;
		this.submitPreferences(preferencesObject, 'defaultProject');
	}

	defaultTaskOnChange(): void {
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['defaultTaskId'] = this.taskModel.id;
		this.submitPreferences(preferencesObject, 'defaultTask');
	}

	dateFormatOnChange(): void {
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['dateFormat'] = this.dateFormatModel.dateFormat;
		preferencesObject['dateFormatId'] = this.dateFormatModel.dateFormatId;
		this.submitPreferences(preferencesObject, 'dateFormat');
	}

	timeFormatOnChange(): void {
		this.setSendEmailTimeData(this.timeFormatModel.timeFormat);
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['timeFormat'] = this.timeFormatModel.timeFormat;
		this.submitPreferences(preferencesObject, 'timeFormat');
	}

	weekStartDayOnChange(): void {
		this.setSendEmailWeekDaysArray(this.userModel.weekStart);
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['weekStart'] = this.weekStartDays.indexOf(this.weekStartDayModel);
		this.submitPreferences(preferencesObject, 'weekStartDay');
	}

	isWeeklyTimeEntryUpdatesSendOnChange(): void {
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['isWeeklyTimeEntryUpdatesSend'] = this.userModel.isWeeklyTimeEntryUpdatesSend;
		this.submitPreferences(preferencesObject, 'isWeeklyTimeEntryUpdatesSend');
	}

	workingHoursPerDayOnChange(): void {
		const preferencesObject = this.getPreferencesObject();
		preferencesObject['workingHoursPerDay'] = this.userModel.workingHoursPerDay;
		this.submitPreferences(preferencesObject, 'workingHoursPerDay');
	}

	// SUBMIT

	submitPersonalInfo(fieldChanged: string): void {
		if (this.personalInfoForm.invalid) {
			return;
		}

		const personalInfoObject = {
			email: this.user.email,
			fullName: this.user.fullName
		};
		personalInfoObject[fieldChanged] = this.userModel[fieldChanged];

		this.personalInfoFormStatus[fieldChanged] = 'loading';
		this.profileService.submitPersonalInfo(personalInfoObject, this.userModel.id)
			.subscribe((userModel: User) => {
					this.personalInfoFormStatus[fieldChanged] = 'success';
					if (this.isEmailChanged && this.isGravatarIcon(this.avatarUrl)) {
						this.getAvatar().then((avatarUrl) => this.updateAvatar(avatarUrl));
					}

					this.isEmailChanged = false;
					this.userModel.email = userModel.email;

					if (this.impersonationService.impersonationId) {
						let impersonateUser = Object.assign(this.impersonationService.impersonationUser, personalInfoObject);
						this.impersonationService.impersonationUser = impersonateUser;
						this.impersonationService.setStorage(impersonateUser);
						this.impersonationService.onChange.emit(impersonateUser);
					} else {
						this.usersService.setUserInfo(personalInfoObject);
					}
				},
				errResponse => {
					this.personalInfoFormStatus[fieldChanged] = 'error';
					if (errResponse.error.includes('Duplicate email.')) {
						this.showWrongEmailMessage = true;
					}
				});
	}

	submitPreferences(preferencesObject: any, fieldChanged: string): void {
		if (this.preferencesForm.invalid) {
			return;
		}

		this.preferencesFormStatus[fieldChanged] = 'loading';
		this.profileService.submitPreferences(preferencesObject, this.userModel.id)
			.subscribe(() => {
					this.preferencesFormStatus[fieldChanged] = 'success';
					if (this.impersonationService.impersonationId) {
						const impersonateUser = Object.assign(this.impersonationService.impersonationUser, preferencesObject);
						this.impersonationService.setStorage(impersonateUser);
					} else {
						this.usersService.setUserInfo(preferencesObject);
					}
				},
				() => {
					this.preferencesFormStatus[fieldChanged] = 'error';
				});
	}

	private getPreferencesObject() {
		return {
			defaultProjectId: this.user.defaultProjectId,
			defaultTaskId: this.user.defaultTaskId,
			dateFormat: this.user.dateFormat,
			dateFormatId: this.user.dateFormatId,
			isWeeklyTimeEntryUpdatesSend: this.user.isWeeklyTimeEntryUpdatesSend,
			timeFormat: this.user.timeFormat,
			weekStart: this.user.weekStart,
			workingHoursPerDay: this.user.workingHoursPerDay
		}
	}

	// GENERAL

	private getProjects(): void {
		this.projectsService.getProjects().subscribe((projects: Project[]) => {
			this.projects = projects;
			this.projects.unshift(new Project({
				id: 0,
				name: 'No project'
			}));

			this.projectModel = this.projects.find((project: Project) => project.id === this.userModel.defaultProjectId) || this.projects[0];
			this.loadTasks(this.projectModel && this.projectModel.id);
		});
	}

	private getDateFormats(): void {
		this.profileService.getDateFormats().subscribe((formats: DateFormat[]) => {
			this.dateFormats = formats;
			this.dateFormatModel = ArrayUtils.findByProperty(this.dateFormats, 'dateFormatId', this.userModel.dateFormatId) || null;
		});
	}

	private loadTasks(projectId?: number): void {
		this.isTasksLoading = true;
		this.tasksService.getActiveTasks(projectId)
			.finally(() => this.isTasksLoading = false)
			.subscribe((res) => {
				this.tasks = this.filterTasks(res.data);
				this.tasks.unshift(new Task({
					id: 0,
					name: 'No task'
				}));
				this.taskModel = ArrayUtils.findByProperty(this.tasks, 'id', this.userModel.defaultTaskId) || this.tasks[0];
			});
	}

	private filterTasks(tasks: Task[]): Task[] {
		const filteredTasks: Task[] = [];
		let isAdded: boolean = false;

		tasks.forEach((task1, index1) => {
			isAdded = false;
			if (task1.projectId) {
				filteredTasks.push(task1);
			} else {
				tasks.forEach((task2, index2) => {
					if (task2.projectId && task1.name.toLowerCase() === task2.name.toLowerCase() && index1 !== index2) {
						isAdded = true;
					}
				});
				if (!isAdded) {
					filteredTasks.push(task1);
				}
			}
		});

		return filteredTasks;
	}

	private setSendEmailTimeData(timeFormat: number): void {
		this.sendEmailTimeArray = timeFormat === 12 ? TWELVE_CLOCK_TIME_ARRAY : STANDART_TIME_ARRAY;
		this.sendEmailTimeModel = this.sendEmailTimeArray[this.userModel.sendEmailTime];
	}

	private setSendEmailWeekDaysArray(weekStartDay: number): void {
		this.sendEmailDaysArray = [];
		NOT_FULL_WEEK_DAYS.forEach((value: string, i: number) => this.sendEmailDaysArray.push(new WeekDay(value, i + 1)));

		if (weekStartDay === 0) {
			this.sendEmailDaysArray.unshift(new WeekDay('Sunday', 0));
		} else {
			this.sendEmailDaysArray.push(new WeekDay('Sunday', 0));
		}

		if (this.userModel.sendEmailDays) {
			this.userModel.sendEmailDays.split(',').forEach((dayNumber: string) => this.sendEmailDays[+dayNumber] = true);
		}
	}
}
