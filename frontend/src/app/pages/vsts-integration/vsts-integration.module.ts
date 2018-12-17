import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { VstsIntegrationFormComponent } from './form/vsts-integration-form.component';
import { VstsIntegrationRoutingModule } from './vsts-integration-routing.module';
import { VstsIntegrationComponent } from './vsts-integration.component';
import { ProjectUsersFormComponent } from './project-users-form/project-users-form.component';
import { VstsIntegrationService } from '../../services/vsts-integration.service';

@NgModule({
	imports: [
		VstsIntegrationRoutingModule,
		SharedModule
	],
	declarations: [
		VstsIntegrationComponent,
		VstsIntegrationFormComponent,
		ProjectUsersFormComponent,
	],
	entryComponents: [
		VstsIntegrationFormComponent,
		ProjectUsersFormComponent,
	],
	providers: [
		VstsIntegrationService
	],
	exports: [
		VstsIntegrationComponent
	]
})

export class VstsIntegrationModule {
}
