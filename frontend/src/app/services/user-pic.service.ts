import { Response } from '@angular/http';
import { Observable } from 'rxjs';
import { EventEmitter, Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';
import { CustomHttp } from '../core/custom-http';

export interface Avatar {
	avatarFileName: string;
	avatarUrl: string;
	memberId: number;
}

@Injectable()
export class UserPicService {
	onUserPicChange: EventEmitter<string> = new EventEmitter<string>();

	constructor(private http: CustomHttp,
	            private constantService: ConstantService) {
	}

	loadUserPicture(userId: number): Observable<Avatar> {
		return this.http.get(this.constantService.profileApi + '/Member(' + userId + ')/UrlAvatar')
			.map((res: Response) => res.json());
	}
}
