import { NgModule } from '@angular/core';
import { TasksRoutingModule } from './tasks-routing.module';
import { TasksComponent } from './tasks.component';
import { TaskFormComponent } from './form/tasks-form.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
	imports: [
		SharedModule,
		TasksRoutingModule
	],
	declarations: [TasksComponent, TaskFormComponent],
	entryComponents: [
		TaskFormComponent
	]
})

export class TasksModule {
}
