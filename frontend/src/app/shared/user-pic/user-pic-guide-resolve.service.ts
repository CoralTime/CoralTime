import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { UserPicService } from '../../services/user-pic.service';

@Injectable()
export class UserPicGuideResolveService implements Resolve<string> {
	constructor(private userPicService: UserPicService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<string> {
		return this.userPicService.getPicturesCacheGuid()
			.toPromise()
			.then((guide: string) => guide);
	}
}
