import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConstantService } from '../core/constant.service';
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
	memberId: number;
	memberName: string;
	urlIcon: string;

	constructor(data: any) {
		if (!data) {
			return;
		}
		this.memberId = data.memberId;
		this.memberName = data.memberName;
		this.urlIcon = data.urlIcon;
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

export class WeekDay {
	name: string;
	dayNumber: number;

	constructor(name: string, dayNumber: number) {
		this.name = name;
		this.dayNumber = dayNumber;
	}
}

export const NOT_FULL_WEEK_DAYS = [
	'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
];

@Injectable()
export class ProfileService {
	constructor(private constantService: ConstantService,
	            private http: HttpClient) {
	}

	getDateFormats(): Observable<DateFormat[]> {
		return this.http.get(this.constantService.profileApi + '/DateFormats')
			.map((res: Object[]) => res.map(x => new DateFormat(x)));
	}

	getProjects(): Observable<ProfileProjects[]> {
		return this.http.get(this.constantService.profileApi + '/Projects')
			.map((res: Object[]) => res ? res.map(x => new ProfileProjects(x)) : []);
	}

	getProjectMembers(projectId: number): Observable<ProfileProjectMember[]> {
		return this.http.get(this.constantService.profileApi + '/ProjectMembers/' + projectId)
			.map((res: Object[]) => res.map(x => new ProfileProjectMember(x)));
	}

	submitNotifications(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/Notifications', obj);
	}

	submitPreferences(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/Preferences', obj);
	}

	submitPersonalInfo(obj: any, userId: number): Observable<any> {
		return this.http.patch(this.constantService.profileApi + '/Member(' + userId + ')/PersonalInfo', obj);
	}
}
