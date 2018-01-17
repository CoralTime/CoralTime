import { ProjectSettingsFormComponent } from './project-settings-form/project-settings-form.component';
import { ProjectFormComponent } from './project-form/project-form.component';
import { SharedModule } from '../../shared/shared.module';
import { ProjectsRoutingModule } from './projects-routing.module';
import { ProjectsComponent } from './projects.component';
import { NgModule } from '@angular/core';
import { ProjectTasksComponent } from './project-tasks-form/project-tasks.component';
import { ProjectUsersComponent } from './project-members-form/project-members.component';
import { ProjectTasksFormComponent } from './project-tasks-form/form/project-tasks-form.component';
import { TaskEqualValidatorDirective } from './project-tasks-form/form/task-equal-validator.directive';

@NgModule({
	imports: [
		ProjectsRoutingModule,
		SharedModule
	],
	declarations: [
		ProjectsComponent,
		ProjectFormComponent,
		ProjectTasksComponent,
		ProjectTasksFormComponent,
		ProjectSettingsFormComponent,
		ProjectUsersComponent,
		TaskEqualValidatorDirective
	],
	entryComponents: [
		ProjectFormComponent,
		ProjectTasksComponent,
		ProjectUsersComponent,
		ProjectSettingsFormComponent
	],
	exports: [
		ProjectsComponent
	]
})

export class ProjectsModule {
}
