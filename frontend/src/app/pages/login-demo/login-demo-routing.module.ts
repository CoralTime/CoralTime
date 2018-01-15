import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NotAuthGuard } from '../../core/auth/not-auth-guard.service';
import { LoginDemoComponent } from './login-demo.component';

const routes: Routes = [
	{
		path: '',
		component: LoginDemoComponent,
		canActivate: [NotAuthGuard]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes)
	],
	exports: [
		RouterModule
	]
})

export class LoginDemoRoutingModule {
}