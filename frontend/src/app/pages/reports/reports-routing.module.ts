import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from '../../core/auth/auth-guard.service';
import { ReportsComponent } from './reports.component';
import { UserInfoResolve } from '../../core/auth/user-info-resolve.service';
import { ReportFiltersResolveService } from './report-filters-resolve.service';

const routes: Routes = [
	{
		path: '',
		component: ReportsComponent,
		canActivate: [AuthGuard],
		data: {
			title: 'Reports',
			role: 'roleViewProject'
		},
		resolve: {
			reportFilters: ReportFiltersResolveService,
			user: UserInfoResolve
		}
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [
		ReportFiltersResolveService,
		UserInfoResolve
	]
})

export class ReportsRoutingModule {
}
