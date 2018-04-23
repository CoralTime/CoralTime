import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ForbiddenComponent } from './pages/errors/components/forbidden/forbidden.component';
import { ServerErrorComponent } from './pages/errors/components/server-error/server-error.component';

export const appRoutes: Routes = [
	{
		path: '',
		redirectTo: 'calendar',
		pathMatch: 'full'
	},
	{
		path: 'home',
		loadChildren: 'app/pages/home/home.module#HomeModule'
	},
	{
		path: 'signin-oidc',
		loadChildren: 'app/pages/signin-oidc/signin-oidc.module#SignInOidcModule'
	},
	{
		path: 'profile',
		loadChildren: 'app/pages/profile/profile.module#ProfileModule'
	},
	{
		path: 'projects',
		loadChildren: 'app/pages/projects/projects.module#ProjectsModule'
	},
	{
		path: 'clients',
		loadChildren: 'app/pages/clients/clients.module#ClientsModule'
	},
	{
		path: 'calendar',
		loadChildren: 'app/pages/calendar/calendar.module#CalendarModule'
	},
	{
		path: 'about',
		loadChildren: 'app/pages/about/about.module#AboutModule'
	},
	{
		path: 'login',
		loadChildren: 'app/pages/login/login.module#LoginModule'
	},
	{
		path: 'set-password',
		loadChildren: 'app/pages/set-password/set-password.module#SetPasswordModule'
	},
	{
		path: 'tasks',
		loadChildren: 'app/pages/tasks/tasks.module#TasksModule'
	},
	{
		path: 'users',
		loadChildren: 'app/pages/users/users.module#UsersModule'
	},
	{
		path: 'reports',
		loadChildren: 'app/pages/reports/reports.module#ReportsModule'
	},
	{
		path: 'error',
		component: ServerErrorComponent
	},
	{
		path: 'page-not-found',
		component: ForbiddenComponent
	},
	{
		path: '**',
		component: ForbiddenComponent
	}
];

@NgModule({
	imports: [
		RouterModule.forRoot(appRoutes)
	],
	exports: [
		RouterModule
	]
})

export class AppRoutingModule {
}
