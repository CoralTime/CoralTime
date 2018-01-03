import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProjectsComponent } from './projects.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { UserPicGuideResolveService } from '../../shared/user-pic/user-pic-guide-resolve.service';

const routes: Routes = [
	{
		path: '',
		component: ProjectsComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Projects',
			role: 'roleViewProject'
		},
		resolve: {
			userPicGuide: UserPicGuideResolveService
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [UserPicGuideResolveService]
})

export class ProjectsRoutingModule {
}
