import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SignInOidcComponent } from './signin-oidc.component';

const routes: Routes = [
	{
		path: '',
		component: SignInOidcComponent,
		data: {title: 'SignInOidc'}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: []
})

export class SignInOidcRoutingModule {
}
