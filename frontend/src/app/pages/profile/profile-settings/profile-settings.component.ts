import { Component, OnInit } from '@angular/core';
import { ImpersonationService } from '../../../services/impersonation.service';
import { User } from '../../../models/user';
import { Project } from '../../../models/project';
import { Task } from '../../../models/task';
import { ProjectsService } from '../../../services/projects.service';
import { TasksService } from '../../../services/tasks.service';
import { DateFormat, NOT_FULL_WEEK_DAYS, ProfileService, TimeFormat, WeekDay } from '../../../services/profile.service';
import { EnterEmailService } from '../../set-password/enter-email/enter-email.service';
import { ArrayUtils } from '../../../core/object-utils';
import { NotificationService } from '../../../core/notification.service';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ProfilePhotoComponent } from './profile-photo/profile-photo.component';
import { SelectComponent } from '../../../shared/form/select/select.component';
import { EMAIL_PATTERN } from '../../../core/constant.service';
import { ActivatedRoute } from '@angular/router';
import { UserPicService } from '../../../services/user-pic.service';
import { UsersService } from '../../../services/users.service';

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
	userModel: User = new User();

	avatarUrl: string;
	dateFormats: DateFormat[];
	dateFormatModel: DateFormat;
	emailPattern = EMAIL_PATTERN;
	isEmailChanged: boolean;
	isFormShownArray: boolean[] = [true, true, true];
	projects: Project[];
	projectModel: Project;
	resetPasswordMessage: string;
	sendEmailDaysArray: WeekDay[];
	sendEmailDays: boolean[] = [];
	sendEmailTimeModel: string;
	sendEmailTimeArray: string[];
	showWrongEmailMessage: boolean = false;
	tasks: Task[];
	taskModel: Task;
	timeFormats: TimeFormat[] = [new TimeFormat(12), new TimeFormat(24)];
	timeFormatModel: TimeFormat = this.timeFormats[1];
	weekStartDays: string[] = ['Sunday', 'Monday'];
	weekStartDayModel: string;

	private dialogRef: MatDialogRef<ProfilePhotoComponent>;

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
			this.userModel = new User(this.impersonationService.impersonationUser || data.user);
		});

		this.avatarUrl = this.userModel.urlIcon.replace('Icons', 'Avatars');
		this.timeFormatModel = this.userModel.timeFormat ? new TimeFormat(this.userModel.timeFormat) : this.timeFormats[1];
		this.weekStartDayModel = this.weekStartDays[this.userModel.weekStart];

		this.getAvatar();
		this.getDateFormats();
		this.getProjects();
		this.setSendEmailTimeData(this.userModel.timeFormat);
		this.setSendEmailWeekDaysArray(this.userModel.weekStart);
	}

	// GENERAL

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

	toggleForm(formIndex: number): void {
		this.isFormShownArray[formIndex] = !this.isFormShownArray[formIndex];
	}

	updateAvatar(avatarUrl: string): void {
		this.avatarUrl = avatarUrl;
		let iconObject = {
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

	dateFormatOnChange(dateFormat: DateFormat): void {
		this.userModel.dateFormatId = dateFormat.dateFormatId;
		this.userModel.dateFormat = dateFormat.dateFormat;
	}

	defaultProjectOnChange(project: Project): void {
		this.userModel.defaultProjectId = project.id;
		this.loadTasks(project.id);
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
			}, error => this.notificationService.danger('Error when resetting the password.'));
	}

	sendEmailDayOnChange(): void {
		let sendEmailDays = [];
		this.sendEmailDays.forEach((dayChecked: boolean, i: number) => {
			if (dayChecked) {
				sendEmailDays.push(i);
			}
		});

		this.userModel.sendEmailDays = sendEmailDays.join(',');
	}

	sendEmailTimeOnChange(select: SelectComponent): void {
		this.userModel.sendEmailTime = select.getOptionIndex(this.sendEmailTimeModel);
	}

	timeFormatOnChange(): void {
		this.userModel.timeFormat = this.timeFormatModel.timeFormat;
		this.setSendEmailTimeData(this.timeFormatModel.timeFormat);
	}

	weekStartDayOnChange(): void {
		this.userModel.weekStart = this.weekStartDays.indexOf(this.weekStartDayModel);
		this.setSendEmailWeekDaysArray(this.userModel.weekStart);
	}

	// SUBMIT

	submitNotifications(): void {
		let notificationsObject = {
			isWeeklyTimeEntryUpdatesSend: this.userModel.isWeeklyTimeEntryUpdatesSend,
			sendEmailTime: this.userModel.sendEmailTime,
			sendEmailDays: this.userModel.sendEmailDays
		};

		this.profileService.submitNotifications(notificationsObject, this.userModel.id)
			.subscribe(() => {
					if (this.impersonationService.impersonationId) {
						let impersonateUser = Object.assign(this.impersonationService.impersonationUser, notificationsObject);
						this.impersonationService.setStorage(impersonateUser);
					} else {
						this.usersService.setUserInfo(notificationsObject);
					}

					this.notificationService.success('Profile settings has been successfully changed.');
				},
				error => {
					this.notificationService.danger('Error changing profile settings.');
				});
	}

	submitPersonalInfo(): void {
		this.showWrongEmailMessage = false;
		let personalInfoObject = {
			email: this.userModel.email,
			fullName: this.userModel.fullName
		};

		this.profileService.submitPersonalInfo(personalInfoObject, this.userModel.id)
			.subscribe((userModel: User) => {
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

					this.notificationService.success('Profile settings has been successfully changed.');
				},
				errResponse => {
					if (errResponse.error.includes('Duplicate email.')) {
						this.showWrongEmailMessage = true;
					}

					this.notificationService.danger('Error changing profile settings.');
				});
	}

	submitPreferences(): void {
		let preferencesObject = {
			defaultProjectId: this.userModel.defaultProjectId,
			defaultTaskId: this.userModel.defaultTaskId,
			dateFormat: this.userModel.dateFormat,
			dateFormatId: this.userModel.dateFormatId,
			isWeeklyTimeEntryUpdatesSend: this.userModel.isWeeklyTimeEntryUpdatesSend,
			timeFormat: this.userModel.timeFormat,
			weekStart: this.userModel.weekStart
		};

		this.profileService.submitPreferences(preferencesObject, this.userModel.id)
			.subscribe(() => {
					if (this.impersonationService.impersonationId) {
						let impersonateUser = Object.assign(this.impersonationService.impersonationUser, preferencesObject);
						this.impersonationService.setStorage(impersonateUser);
					} else {
						this.usersService.setUserInfo(preferencesObject);
					}

					this.notificationService.success('Profile settings has been successfully changed.');
				},
				error => {
					this.notificationService.danger('Error changing profile settings.');
				});
	}

	private getProjects(): void {
		this.projectsService.getProjects().subscribe((projects: Project[]) => {
			this.projects = projects;
			this.projects.unshift(new Project({
				id: 0,
				name: 'No project'
			}));

			this.projectModel = this.projects.find((project: Project) => project.id === this.userModel.defaultProjectId) || this.projects[0];
			if (!this.projectModel) {
				this.loadTasks();
			}
		});
	}

	private getDateFormats(): void {
		this.profileService.getDateFormats().subscribe((formats: DateFormat[]) => {
			this.dateFormats = formats;
			this.dateFormatModel = ArrayUtils.findByProperty(this.dateFormats, 'dateFormatId', this.userModel.dateFormatId) || null;
		});
	}

	private loadTasks(projectId?: number): void {
		this.tasksService.getActiveTasks(projectId).subscribe((res) => {
			this.tasks = this.filterTasks(res.data);
			this.tasks.unshift(new Task({
				id: 0,
				name: 'No task'
			}));
			this.taskModel = ArrayUtils.findByProperty(this.tasks, 'id', this.userModel.defaultTaskId) || this.tasks[0];
		});
	}

	private filterTasks(tasks: Task[]): Task[] {
		let filteredTasks: Task[] = [];
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
