import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AboutComponent } from './about.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';

const routes: Routes = [
	{
		path: '',
		component: AboutComponent,
		canActivate: [AuthGuard]
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: []
})

export class AboutRoutingModule {
}
