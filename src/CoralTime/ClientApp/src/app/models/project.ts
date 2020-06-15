import { GRAY_COLOR, hexToNumber } from '../shared/form/color-picker/color-picker.component';

export class Project {
	clientId: number;
	clientName: string;
	clientIsActive: boolean;
	color: number;
	daysBeforeStopEditTimeEntries: number;
	description: string;
	id: number;
	isActive: boolean;
	isCurrentUserOnProject: boolean;
	isNotificationEnabled: boolean;
	isPrivate: boolean;
	isTimeLockEnabled: boolean;
	lockPeriod: number;
	membersCount: number;
	name: string;
	notificationDay: number;
	tasksCount: number;

	constructor(data = null) {
		this.clientId = data && data.clientId;
		this.clientName = data && data.clientName;
		this.clientIsActive = data && data.clientIsActive;
		this.color = data && data.color || hexToNumber(GRAY_COLOR);
		this.daysBeforeStopEditTimeEntries = data && data.daysBeforeStopEditTimeEntries;
		this.description = data && data.description;
		this.id = data && data.id;
		this.isActive = data && data.isActive;
		this.isCurrentUserOnProject = data && data.isCurrentUserOnProject;
		this.isNotificationEnabled = data && data.isNotificationEnabled;
		this.isPrivate = data && data.isPrivate;
		this.isTimeLockEnabled = data && data.isTimeLockEnabled;
		this.lockPeriod = data && data.lockPeriod;
		this.notificationDay = data && data.notificationDay;
		this.membersCount = data && data.membersCount;
		this.name = data && data.name;
		this.tasksCount = data && data.tasksCount;
	}
}
