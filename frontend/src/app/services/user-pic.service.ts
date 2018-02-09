import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { EventEmitter, Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';

export interface Avatar {
	avatarFileName: string;
	avatarUrl: string;
	memberId: number;
}

export interface UserPicCache {
	userPicCacheGuide: string;
	userPictures: Avatar[];
}

const USER_PIC_CASH = 'USER_PIC_CASH';

@Injectable()
export class UserPicService {
	onUserPicChange: EventEmitter<void> = new EventEmitter<void>();
	private userAvatar: Avatar;

	constructor(private http: Http,
	            private constantService: ConstantService) {
	}

	getUserPicture(userId: number, isAvatar: boolean): Observable<Avatar> {
		if (this.userAvatar) {
			return Observable.of(this.userAvatar);
		}

		return this.loadUserPicture(userId, isAvatar);
	}

	loadUserPicture(userId: number, isAvatar: boolean): Observable<Avatar> {
		return this.http.get((isAvatar ? this.constantService.userAvatarApi : this.constantService.userIconApi) + userId)
			.map((res: Response) => {
				this.userAvatar = res.json();
				return this.userAvatar;
			});
	}
}
