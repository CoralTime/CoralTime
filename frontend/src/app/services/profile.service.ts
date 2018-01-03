import { Http, Response } from '@angular/http';
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
		return this.timeFormat == 12 ? this.timeFormat + ' hours (Ex: 3:00 PM)' : this.timeFormat + ' hours (Ex: 15:00)';
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
	fullName: string;
	userId: number;

	constructor(data: any) {
		if (!data) {
			return;
		}
		this.fullName = data.MemberName;
		this.userId = data.MemberId;
	}
}

@Injectable()
export class ProfileService {
	constructor(private http: Http,
	            private constantService: ConstantService) {
	}

	getDateFormats(): Observable<DateFormat[]> {
		return this.http.get(this.constantService.profileApi + '/DateFormats')
			.map((res: Response) => res.json().map(x => new DateFormat(x)))
	}

	getTimeZones(): TimeZone[] {
		let timeZones = [];
		for (let key in TIME_ZONES) {
			timeZones.push(new TimeZone(key, TIME_ZONES[key]))
		}

		return timeZones;
	}

	upload(fileToUpload: File): Observable<any> {
		let input = new FormData();
		input.append("file", fileToUpload, fileToUpload.name);

		return this.http
			.put("/api/v1/Profile", input);
	}

	getProjects(): Observable<ProfileProjects[]> {
		return this.http.get(this.constantService.profileApi + '/Projects')
			.map((res: Response) => {
				return res.json().map(x => new ProfileProjects(x))
			})
	}

	getProjectMembers(projectId: number): Observable<ProfileProjectMember[]> {
		return this.http.get(this.constantService.profileApi + '/ProjectMembers/' + projectId)
			.map((res: Response) => res.json().value.map(x => new ProfileProjectMember(x)))
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
}