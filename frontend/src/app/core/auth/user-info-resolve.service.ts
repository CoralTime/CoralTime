import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../../models/user';
import { AuthService } from './auth.service';
import { UsersService } from '../../services/users.service';

@Injectable()
export class UserInfoResolve implements Resolve<User> {
	constructor(private authService: AuthService,
	            private usersService: UsersService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<User> {
		return this.usersService.getUserInfo(this.authService.authUser.id)
			.then((user: User) => {
				return user;
			});
	}
}
