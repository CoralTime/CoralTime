import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UsersComponent } from './users.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { UserPicGuideResolveService } from '../../shared/user-pic/user-pic-guide-resolve.service';

const routes: Routes = [
	{
		path: '',
		component: UsersComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Users',
			role: 'roleViewMember'
		},
		resolve: {
			userPicGuide: UserPicGuideResolveService
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [UserPicGuideResolveService]
})

export class UsersRoutingModule {
}
