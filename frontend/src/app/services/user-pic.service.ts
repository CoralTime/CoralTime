import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { EventEmitter, Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';

export interface Avatar {
	AvatarFile: string;
	AvatarFileName: string;
	IsAvatar: boolean;
	MemberId: number;
}

export interface UserPicCache {
	userPicCacheGuide: string;
	userPictures: Avatar[];
}

const USER_PIC_CASH = 'USER_PIC_CASH';

@Injectable()
export class UserPicService {
	onUserPicChange: EventEmitter<void> = new EventEmitter<void>();

	private actualUserPicCacheGuide: string;
	private userPicCache: UserPicCache = {
		userPicCacheGuide: '',
		userPictures: []
	};

	constructor(private http: Http,
	            private constantService: ConstantService) {
		if (localStorage.hasOwnProperty(USER_PIC_CASH)) {
			this.userPicCache = JSON.parse(localStorage.getItem(USER_PIC_CASH));
		}
	}

	clearUserPictureCache(): void {
		this.userPicCache = {
			userPicCacheGuide: this.actualUserPicCacheGuide,
			userPictures: []
		};
		localStorage.setItem(USER_PIC_CASH, JSON.stringify(this.userPicCache));
	}

	getPicturesCacheGuid(): Observable<string> {
		return this.http.get(this.constantService.apiBaseUrl + '/PicturesCacheGuid')
			.map((res: Response) => {
				this.actualUserPicCacheGuide = res.json()['picturesCacheGuid'];
				return this.actualUserPicCacheGuide;
			});
	}

	getUserPicture(userId: number, isAvatar: boolean): Observable<string> {
		let userPic: Avatar;

		if (this.userPicCache && this.actualUserPicCacheGuide) {
			if (this.actualUserPicCacheGuide === this.userPicCache.userPicCacheGuide) {
				userPic = this.getUserPicFromCache(userId, isAvatar);
			} else {
				this.clearUserPictureCache();
			}
		}

		if (userPic) {
			return Observable.of(userPic.AvatarFile);
		}
		return this.loadUserPicture(userId, isAvatar);
	}

	private cacheUserPicture(userPic: Avatar, isAvatar: boolean): void {
		this.userPicCache.userPictures = this.userPicCache.userPictures || [];
		userPic.IsAvatar = isAvatar;

		this.userPicCache.userPictures.push(userPic);
		localStorage.setItem(USER_PIC_CASH, JSON.stringify(this.userPicCache));
	}

	getUserPicFromCache(id: Number, isAvatar: boolean = false): Avatar {
		if (!this.userPicCache) {
			return null;
		}

		if (isAvatar) {
			return this.userPicCache.userPictures.filter((icon: Avatar) => icon.MemberId === id && icon.IsAvatar === isAvatar)[0];
		} else {
			return this.userPicCache.userPictures.filter((icon: Avatar) => icon.MemberId === id)[0];
		}
	}

	private loadUserPicture(userId: number, isAvatar: boolean): Observable<string> {
		return this.http.get((isAvatar ? this.constantService.userAvatarApi : this.constantService.userIconApi) + userId)
			.map((res: Response) => {
				let userPic: Avatar = res.json();
				this.cacheUserPicture(userPic, isAvatar);
				return userPic.AvatarFile;
			});
	}
}
