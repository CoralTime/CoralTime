import { SharedModule } from '../../shared/shared.module';
import { NgModule } from '@angular/core';
import { LoginComponent } from './login.component';
import { LoginRoutingModule } from './login-routing.module';
import { LoginService } from './login.service';

@NgModule({
	imports: [
		LoginRoutingModule,
		SharedModule
	],
	declarations: [
		LoginComponent
	],
	providers: [
		LoginService
	]
})

export class LoginModule {
}
