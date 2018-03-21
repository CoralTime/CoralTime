import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TasksComponent } from './tasks.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';

const routes: Routes = [
	{
		path: '',
		component: TasksComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Tasks',
			role: 'roleViewTask'
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class TasksRoutingModule {
}
