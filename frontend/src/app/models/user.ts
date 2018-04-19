export class User {
	dateFormat: string;
	dateFormatId: number;
	defaultProjectId: number;
	defaultTaskId: number;
	email: string;
	fullName: string;
	id: number;
	isActive: boolean;
	isAdmin: boolean;
	isManager: boolean;
	isWeeklyTimeEntryUpdatesSend: boolean;
	projectsCount: number;
	sendEmailDays: string;
	sendEmailTime: number;
	timeFormat: number;
	urlIcon: string;
	userName: string;
	weekStart: number;

	constructor(data = null) {
		if (data) {
			this.dateFormat = data.dateFormat;
			this.dateFormatId = data.dateFormatId;
			this.defaultProjectId = data.defaultProjectId;
			this.defaultTaskId = data.defaultTaskId;
			this.email = data.email;
			this.fullName = data.fullName;
			this.id = data.id;
			this.isAdmin = data.isAdmin;
			this.isActive = data.isActive;
			this.isManager = data.isManager;
			this.isWeeklyTimeEntryUpdatesSend = data.isWeeklyTimeEntryUpdatesSend;
			this.projectsCount = data.projectsCount;
			this.sendEmailDays = data.sendEmailDays;
			this.sendEmailTime = data.sendEmailTime;
			this.timeFormat = data.timeFormat;
			this.urlIcon = data.urlIcon;
			this.userName = data.userName;
			this.weekStart = data.weekStart;
		}
	}

	getRole(): string {
		return this.isAdmin ? 'Admin' : 'User';
	}
}
