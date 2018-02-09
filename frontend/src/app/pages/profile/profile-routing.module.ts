import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProfileComponent } from './profile.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { ProfileSettingsComponent } from './profile-settings/profile-settings.component';

const routes: Routes = [
	{
		path: '',
		component: ProfileComponent,
		canActivate: [AuthGuard],
		data: {title: 'Profile'}
	},
	{
		path: 'settings',
		component: ProfileSettingsComponent,
		canActivate: [AuthGuard]
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class ProfileRoutingModule {
}
