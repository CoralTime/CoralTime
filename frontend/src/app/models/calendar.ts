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

export interface TimeOptions {
	isFromToShow: boolean;
	timeTimerStart: number;
}

export interface TimeValues {
	timeActual: number;
	timeEstimated: number;
	timeFrom: number;
	timeTo: number;
}

export class TimeEntry {
	color: number;
	date: string;
	description: string;
	id: number;
	isLocked: boolean;
	isProjectActive: boolean;
	isTaskTypeActive: boolean;
	isUserManagerOnProject: boolean;
	memberId: number;
	memberName: string;
	projectId: number;
	projectName: string;
	taskName: string;
	taskTypesId: number;
	timeOptions: TimeOptions;
	timeValues: TimeValues;

	constructor(data = null) {
		this.color = data && data.color || 0;
		this.date = data && data.date;
		this.description = data && data.description;
		this.id = data && data.id;
		this.isLocked = data && data.isLocked;
		this.isProjectActive = data && data.isProjectActive;
		this.isTaskTypeActive = data && data.isTaskTypeActive;
		this.isUserManagerOnProject = data && data.isUserManagerOnProject;
		this.memberId = data && data.memberId;
		this.memberName = data && data.memberName;
		this.projectId = data && data.projectId;
		this.projectName = data && data.projectName;
		this.taskName = data && data.taskName;
		this.taskTypesId = data && data.taskTypesId;
		this.timeOptions = {
			isFromToShow: data && data.timeOptions && data.timeOptions.isFromToShow,
			timeTimerStart: data && data.timeOptions && data.timeOptions.timeTimerStart || 0
		};
		this.timeValues = {
			timeActual: data && data.timeValues && data.timeValues.timeActual || 1,
			timeEstimated: data && data.timeValues && data.timeValues.timeEstimated || 0,
			timeFrom: data && data.timeValues && data.timeValues.timeFrom,
			timeTo: data && data.timeValues && data.timeValues.timeTo
		};
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
