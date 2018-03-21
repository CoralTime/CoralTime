import { NgModule } from '@angular/core';
import { ClientFormComponent } from './form/client-form.component';
import { ClientsComponent } from './clients.component';
import { ClientsRoutingModule } from './clients-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { ClientProjectAssignmentComponent } from './project-assignment/project-assignment.component';

@NgModule({
	imports: [
		ClientsRoutingModule,
		SharedModule
	],
	declarations: [
		ClientsComponent,
		ClientFormComponent,
		ClientProjectAssignmentComponent
	],
	entryComponents: [
		ClientFormComponent,
		ClientProjectAssignmentComponent
	],
	exports: [
		ClientsComponent
	]
})

export class ClientsModule {
}
