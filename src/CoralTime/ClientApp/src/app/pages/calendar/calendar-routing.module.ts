import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../core/auth/auth-guard.service';
import { CalendarComponent } from './calendar.component';
import { CalendarDailyViewComponent } from './calendar-views/daily-view/daily-view.component';
import { CalendarWeeklyViewComponent } from './calendar-views/weekly-view/weekly-view.component';
import { UserInfoResolve } from '../../core/auth/user-info-resolve.service';

const routes: Routes = [
	{
		path: '',
		component: CalendarComponent,
		data: {
			title: 'Calendar'
		},
		resolve: {
			user: UserInfoResolve
		},
		children: [
			{
				path: '',
				component: CalendarWeeklyViewComponent,
				canActivate: [AuthGuard]
			},
			{
				path: 'day',
				component: CalendarDailyViewComponent,
				canActivate: [AuthGuard],
				resolve: {
					user: UserInfoResolve
				}
			},
			{
				path: 'week',
				component: CalendarWeeklyViewComponent,
				canActivate: [AuthGuard],
				resolve: {
					user: UserInfoResolve
				}
			}
		]
	}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: [UserInfoResolve]
})

export class CalendarRoutingModule {
}
