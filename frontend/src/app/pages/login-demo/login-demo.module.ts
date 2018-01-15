import { SharedModule } from '../../shared/shared.module';
import { NgModule } from '@angular/core';
import { LoginDemoComponent } from './login-demo.component';
import { LoginDemoRoutingModule } from './login-demo-routing.module';
import { LoginService } from '../login/login.service';

@NgModule({
	imports: [
		LoginDemoRoutingModule,
		SharedModule
	],
	declarations: [
		LoginDemoComponent
	],
	providers: [
		LoginService
	]
})

export class LoginDemoModule {
}