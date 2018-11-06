import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { ProjectsComponent } from './projects.component';

const routes: Routes = [
	{
		path: '',
		component: ProjectsComponent,
		canActivate: [AuthGuard],
		data: {
			role: 'roleViewProject'
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class ProjectsRoutingModule {
}
