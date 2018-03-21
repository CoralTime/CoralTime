import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ClientsComponent } from './clients.component';
import { AuthGuard } from '../../core/auth/auth-guard.service';

const routes: Routes = [
	{
		path: '',
		component: ClientsComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Clients',
			role: 'roleViewClient'
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule]
})

export class ClientsRoutingModule {
}
