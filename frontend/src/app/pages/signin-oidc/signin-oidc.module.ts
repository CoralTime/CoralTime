import { NgModule } from '@angular/core';
import { SignInOidcRoutingModule } from './signin-oidc-routing.module';
import { SignInOidcComponent } from './signin-oidc.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
	imports: [
		SignInOidcRoutingModule,
		SharedModule
	],
	declarations: [
		SignInOidcComponent
	]
})

export class SignInOidcModule {
}
