import '../vendors'
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ClientsService } from './services/clients.service';
import { ErrorsModule } from './pages/errors/errors.module';
import { SharedModule } from './shared/shared.module';
import { ProjectRolesService } from './services/project-roles.service';
import { CoreModule } from './core/core.module';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { LayoutModule } from './pages/layout/layout.module';
import { ProjectsService } from './services/projects.service';
import { TasksService } from './services/tasks.service';
import { UsersService } from './services/users.service';
import { SettingsService } from './services/settings.service';
import { ImpersonationService } from './services/impersonation.service';

export function httpFactory(http: HttpClient) {
	return new TranslateHttpLoader(http, 'assets/translate/i18n', '.json');
}

@NgModule({
	declarations: [
		AppComponent
	],
	imports: [
		AppRoutingModule,
		BrowserModule,
		BrowserAnimationsModule,
		CoreModule,
		ErrorsModule,
		LayoutModule,
		SharedModule,
		TranslateModule.forRoot({
			loader: {
				provide: TranslateLoader,
				useFactory: httpFactory,
				deps: [HttpClient]
			}
		})
	],
	bootstrap: [AppComponent],
	providers: [
		ClientsService,
		ImpersonationService,
		ProjectRolesService,
		ProjectsService,
		SettingsService,
		TasksService,
		UsersService
	]
})

export class AppModule {
}
