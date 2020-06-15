import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from '../../core/auth/auth-guard.service';
import { VstsIntegrationComponent } from './vsts-integration.component';

const routes: Routes = [
	{
		path: '',
		component: VstsIntegrationComponent,
		canActivate: [AuthGuard],
		data: {
			policy: 'ViewIntegrationPage'
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class VstsIntegrationRoutingModule {
}
