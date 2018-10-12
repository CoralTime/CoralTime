import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { ClientsComponent } from './clients.component';

const routes: Routes = [
	{
		path: '',
		component: ClientsComponent,
		canActivate: [AuthGuard],
		data: {
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
