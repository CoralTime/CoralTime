import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { UsersComponent } from './users.component';

const routes: Routes = [
	{
		path: '',
		component: UsersComponent,
		canActivate: [AuthGuard],
		data: {
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
