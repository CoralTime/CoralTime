import * as moment from 'moment';
import Moment = moment.Moment;

export class CalendarDay {
	date: string;
	timeEntries: TimeEntry[];
	plannedTime: number;
	trackedTime: number;

	constructor(data = null) {
		if (!data) {
			return;
		}
		this.date = data.date;
		this.timeEntries = data.timeEnries || [];
		this.plannedTime = data.plannedTime || 0;
		this.trackedTime = data.trackedTime || 0;
	}
}

export class Time {
	hours: string;
	minutes: string;
	seconds: string;

	constructor(hours: string, minutes: string, seconds?: string) {
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
	}
}

export class TimeEntry {
	color: number;
	date: string;
	description: string;
	id: number;
	isFromToShow: boolean;
	isLocked: boolean;
	isProjectActive: boolean;
	isTaskTypeActive: boolean;
	isUserManagerOnProject: boolean;
	memberId: number;
	memberName: string;
	plannedTime: number;
	projectId: number;
	projectName: string;
	taskName: string;
	taskTypesId: number;
	time: number;
	timeFrom: number;
	timeTimerStart: number;
	timeTo: number;

	constructor(data = null) {
		this.color = data ? (data.color ? data.color : 0) : 0;
		this.date = data ? data.date : null;
		this.description = data ? data.description : null;
		this.id = data ? data.id : null;
		this.isFromToShow = data ? data.isFromToShow : null;
		this.isLocked = data ? data.isLocked : null;
		this.isProjectActive = data ? data.isProjectActive : null;
		this.isTaskTypeActive = data ? data.isTaskTypeActive : null;
		this.isUserManagerOnProject = data ? data.isUserManagerOnProject : null;
		this.memberId = data ? data.memberId : null;
		this.memberName = data ? data.memberName : null;
		this.plannedTime = data ? (data.plannedTime ? data.plannedTime : 0) : 0;
		this.projectId = data ? data.projectId : null;
		this.projectName = data ? data.projectName : null;
		this.taskName = data ? data.taskName : null;
		this.taskTypesId = data ? data.taskTypesId : null;
		this.time = data ? (data.time ? data.time : 1) : 1;
		this.timeFrom = data ? (data.timeFrom >= 0 ? data.timeFrom : null) : null;
		this.timeTo = data ? (data.timeTo >= 0 ? data.timeTo : null) : null;
		this.timeTimerStart = data ? (data.timeTimerStart ? data.timeTimerStart : 0) : 0;
	}
}

export class DateUtils {
	static isToday(date: Date | string): boolean {
		return moment().format('YYYY-MM-DD') === moment(date).format('YYYY-MM-DD');
	}

	static getSecondsFromStartDay(isUTC?: boolean): number {
		let d = new Date();

		if (isUTC) {
			return d.getHours() * 3600 + d.getMinutes() * 60 + d.getSeconds() + d.getTimezoneOffset() * 60;
		}

		return d.getHours() * 3600 + d.getMinutes() * 60 + d.getSeconds();
	}

	static convertMomentToUTC(moment: Moment): Date {
		let date = moment.toDate();
		return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
	}

	static convertMomentToUTCMoment(m: Moment): Moment {
		return moment(this.convertMomentToUTC(m));
	}

	static formatDateToString(d: any): string {
		return moment(d).format('YYYY-MM-DD');
	}

	static formatStringToDate(d: string): Date {
		return moment(d).toDate();
	}

	static reformatDate(d: string, format: string): string {
		return moment(d, format).format('YYYY-MM-DD');
	}
}
