import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { TasksComponent } from './tasks.component';

const routes: Routes = [
	{
		path: '',
		component: TasksComponent,
		canActivate: [AuthGuard],
		data: {
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
