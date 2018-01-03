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
		if (!data) {
			return;
		}

		this.clientId = data.clientId;
		this.clientName = data.clientName;
		this.clientIsActive = data.clientIsActive;
		this.color = data.color ? data.color : 0;
		this.daysBeforeStopEditTimeEntries = data.daysBeforeStopEditTimeEntries;
		this.description = data.description;
		this.id = data.id;
		this.isActive = data.isActive;
		this.isCurrentUserOnProject = data.isCurrentUserOnProject;
		this.isNotificationEnabled = data.isNotificationEnabled;
		this.isPrivate = data.isPrivate;
		this.isTimeLockEnabled = data.isTimeLockEnabled;
		this.lockPeriod = data.lockPeriod;
		this.notificationDay = data.notificationDay;
		this.membersCount = data.membersCount;
		this.name = data.name;
		this.tasksCount = data.tasksCount;
	}
}
