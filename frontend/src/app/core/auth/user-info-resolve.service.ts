import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../../models/user';
import { AuthService } from './auth.service';
import { UsersService } from '../../services/users.service';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Injectable()
export class UserInfoResolve implements Resolve<User> {
	constructor(private authService: AuthService,
	            private loadingService: LoadingMaskService,
	            private usersService: UsersService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<User> {
		this.loadingService.addLoading();
		return this.usersService.getUserInfo(this.authService.authUser.id)
			.then((user: User) => {
				this.loadingService.removeLoading();
				return user;
			});
	}
}
