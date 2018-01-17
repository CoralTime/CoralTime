import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './login.component';
import { NotAuthGuard } from '../../core/auth/not-auth-guard.service';
import { LoginResolve } from './login-resolve.service';

const routes: Routes = [
	{
		path: '',
		component: LoginComponent,
		canActivate: [NotAuthGuard],
		resolve: {loginSettings: LoginResolve}
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes)
	],
	exports: [
		RouterModule
	],
	providers: [
		LoginResolve
	]
})

export class LoginRoutingModule {
}
