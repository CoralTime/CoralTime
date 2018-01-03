import { NgModule } from '@angular/core';
import { ErrorsRoutingModule } from './errors-routing.module';
import { ErrorsComponent } from './errors.component';
import { MessagesModule } from 'primeng/primeng';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
	imports: [
		SharedModule,
		ErrorsRoutingModule,
		MessagesModule
	],
	declarations: [
		ErrorsComponent,
		UnauthorizedComponent,
		ForbiddenComponent
	],
	exports: [
		UnauthorizedComponent,
		ForbiddenComponent
	]
})

export class ErrorsModule {
}