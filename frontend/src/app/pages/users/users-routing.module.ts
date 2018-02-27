import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UsersComponent } from './users.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';

const routes: Routes = [
	{
		path: '',
		component: UsersComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Users',
			role: 'roleViewMember'
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class UsersRoutingModule {
}
