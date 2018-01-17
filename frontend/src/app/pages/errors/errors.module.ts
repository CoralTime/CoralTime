import { NgModule } from '@angular/core';
import { ErrorsComponent } from './errors.component';
import { MessagesModule } from 'primeng/primeng';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';
import { SharedModule } from '../../shared/shared.module';
import { ServerErrorComponent } from './components/server-error/server-error.component';
import { RouterModule } from '@angular/router';

@NgModule({
	imports: [
		SharedModule,
		RouterModule,
		MessagesModule
	],
	declarations: [
		ErrorsComponent,
		UnauthorizedComponent,
		ForbiddenComponent,
		ServerErrorComponent
	],
	exports: [
		UnauthorizedComponent,
		ForbiddenComponent,
		ServerErrorComponent
	]
})

export class ErrorsModule {
}
