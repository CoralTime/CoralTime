import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { EventEmitter, Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';

export interface Avatar {
	avatarFileName: string;
	avatarUrl: string;
	memberId: number;
}

@Injectable()
export class UserPicService {
	onUserPicChange: EventEmitter<string> = new EventEmitter<string>();

	constructor(private http: Http,
	            private constantService: ConstantService) {
	}

	loadUserPicture(userId: number, isAvatar: boolean): Observable<Avatar> {
		return this.http.get((isAvatar ? this.constantService.userAvatarApi : this.constantService.userIconApi) + userId)
			.map((res: Response) => res.json());
	}
}
