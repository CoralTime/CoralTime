import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../../models/user';
import { UserInfoService } from './user-info.service';
import { AuthService } from './auth.service';

@Injectable()
export class UserInfoResolve implements Resolve<User> {
	constructor(private authService: AuthService,
	            private userInfoService: UserInfoService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<User> {
		return this.userInfoService.getUserInfo(this.authService.getAuthUser().id)
			.then((user: User) => {
				return user;
			});
	}
}