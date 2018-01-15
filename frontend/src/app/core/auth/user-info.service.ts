import { EventEmitter, Injectable } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';
import { ObjectUtils } from '../object-utils';

const USER_INFO = 'USER_INFO';

@Injectable()
export class UserInfoService {
	onChange: EventEmitter<User> = new EventEmitter<User>();
	private userInfo: User;

	constructor(private usersService: UsersService) {
		if (localStorage.hasOwnProperty(USER_INFO)) {
			this.userInfo = JSON.parse(localStorage.getItem(USER_INFO));
		}
	}

	getUserInfo(userId: number): Promise<User> {
		if (this.userInfo) {
			return Promise.resolve(this.userInfo);
		}

		return this.loadUserInfo(userId);
	}

	setUserInfo(obj: any): void {
		this.userInfo = (obj && this.userInfo) ? Object.assign(this.userInfo, obj) : obj;
		localStorage.setItem(USER_INFO, JSON.stringify(this.userInfo));
		this.onChange.emit(this.userInfo);
	}

	private loadUserInfo(userId: number): Promise<User> {
		return this.usersService.getUserById(userId)
			.toPromise()
			.then((user: User) => {
				this.setUserInfo(user);
				return this.userInfo
			});
	}
}