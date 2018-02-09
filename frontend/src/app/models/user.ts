export class User {
	dateFormat: string;
	dateFormatId: number;
	defaultProjectId: number;
	defaultTaskId: number;
	email: string;
	fullName: string;
	iconUrl: string;
	id: number;
	isActive: boolean;
	isAdmin: boolean;
	isManager: boolean;
	isWeeklyTimeEntryUpdatesSend: boolean;
	password: string;
	projectsCount: number;
	sendEmailDays: string;
	sendEmailTime: number;
	sendInvitationEmail: boolean;
	timeFormat: number;
	timeZone: string;
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
			this.iconUrl = data.iconUrl;
			this.id = data.id;
			this.isAdmin = data.isAdmin;
			this.isActive = data.isActive;
			this.isManager = data.isManager;
			this.isWeeklyTimeEntryUpdatesSend = data.isWeeklyTimeEntryUpdatesSend;
			this.password = data.password;
			this.projectsCount = data.projectsCount;
			this.sendEmailDays = data.sendEmailDays;
			this.sendEmailTime = data.sendEmailTime;
			this.sendInvitationEmail = data.sendInvitationEmail;
			this.timeFormat = data.timeFormat;
			this.timeZone = data.timeZone;
			this.userName = data.userName;
			this.weekStart = data.weekStart;
		}
	}

	getRole(): string {
		return this.isAdmin ? 'Admin' : 'User';
	}
}
