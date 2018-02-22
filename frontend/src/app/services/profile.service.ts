import { Response } from '@angular/http';
import { CustomHttp } from '../core/custom-http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';
import { TIME_ZONES } from '../pages/profile/profile-settings/timezones';
import * as moment from 'moment';

export class DateFormat {
	dateFormatId: number;
	dateFormat: string;

	constructor(data: any = null) {
		if (data) {
			this.dateFormatId = data.dateFormatId;
			this.dateFormat = data.dateFormat;
		}
	}

	toString(): string {
		return `${this.dateFormat} (Ex: ${moment().format(this.dateFormat)})`;
	}
}

export class TimeFormat {
	timeFormat: number;

	constructor(format: number) {
		this.timeFormat = format;
	}

	toString(): string {
		return this.timeFormat === 12 ? this.timeFormat + ' hours (Ex: 3:00 PM)' : this.timeFormat + ' hours (Ex: 15:00)';
	}
}

export class TimeZone {
	name: string;
	value: string;

	constructor(name: string, value: string) {
		this.name = name;
		this.value = value;
	}
}

export const NOT_FULL_WEEK_DAYS = [
	'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
];

export class WeekDay {
	name: string;
	dayNumber: number;

	constructor(name: string, dayNumber: number) {
		this.name = name;
		this.dayNumber = dayNumber;
	}
}

export class ProfileProjects {
	color: number;
	id: number;
	isMemberListShown: boolean;
	isPrivate: boolean;
	isPrimary: boolean;
	managersNames: string[];
	memberCount: number;
	memberList: ProfileProjectMember[];
	name: string;

	constructor(data: any) {
		if (!data) {
			return;
		}

		this.color = data.color;
		this.id = data.id;
		this.isMemberListShown = false;
		this.isPrivate = data.isPrivate;
		this.isPrimary = data.isPrimary;
		this.managersNames = data.managersNames;
		this.memberCount = data.memberCount;
		this.memberList = [];
		this.name = data.name;
	}
}

export class ProfileProjectMember {
	iconUrl: string;
	memberId: number;
	memberName: string;

	constructor(data: any) {
		if (!data) {
			return;
		}
		this.iconUrl = data.iconUrl;
		this.memberId = data.memberId;
		this.memberName = data.memberName;
	}
}

@Injectable()
export class ProfileService {
	constructor(private http: CustomHttp,
	            private constantService: ConstantService) {
	}

	getDateFormats(): Observable<DateFormat[]> {
		return this.http.get(this.constantService.profileApi + '/DateFormats')
			.map((res: Response) => res.json().map(x => new DateFormat(x)));
	}

	getTimeZones(): TimeZone[] {
		let timeZones = [];
		for (let key in TIME_ZONES) {
			timeZones.push(new TimeZone(key, TIME_ZONES[key]));
		}

		return timeZones;
	}

	getProjects(): Observable<ProfileProjects[]> {
		return this.http.get(this.constantService.profileApi + '/Projects')
			.map((res: Response) => {
				let projects = res.json();
				return projects ? projects.map(x => new ProfileProjects(x)) : [];
			});
	}

	getProjectMembers(projectId: number): Observable<ProfileProjectMember[]> {
		return this.http.get(this.constantService.profileApi + '/ProjectMembers/' + projectId)
			.map((res: Response) => res.json().map(x => new ProfileProjectMember(x)));
	}

	submitNotifications(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/Notifications', obj)
			.map((res: Response) => res.json());
	}

	submitPreferences(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/Preferences', obj)
			.map((res: Response) => res.json());
	}

	submitPersonalInfo(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/PersonalInfo', obj)
			.map((res: Response) => res.json());
	}

	upload(fileToUpload: File): Observable<any> {
		let input = new FormData();
		input.append('file', fileToUpload, fileToUpload.name);

		return this.http.put('/api/v1/Profile', input);
	}
}
