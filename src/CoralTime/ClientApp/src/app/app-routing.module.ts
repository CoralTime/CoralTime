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
		loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule)
	},
	{
		path: 'signin-oidc',
		loadChildren: () => import('./pages/signin-oidc/signin-oidc.module').then(m => m.SignInOidcModule)
	},
	{
		path: 'profile',
		loadChildren: () => import('./pages/profile/profile.module').then(m => m.ProfileModule)
	},
	{
		path: 'projects',
		loadChildren: () => import('./pages/projects/projects.module').then(m => m.ProjectsModule)
	},
	{
		path: 'clients',
		loadChildren: () => import('./pages/clients/clients.module').then(m => m.ClientsModule)
	},
	{
		path: 'calendar',
		loadChildren: () => import('./pages/calendar/calendar.module').then(m => m.CalendarModule)
	},
	{
		path: 'about',
		loadChildren: () => import('./pages/about/about.module').then(m => m.AboutModule)
	},
	{
		path: 'login',
		loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule)
	},
	{
		path: 'set-password',
		loadChildren: () => import('./pages/set-password/set-password.module').then(m => m.SetPasswordModule)
	},
	{
		path: 'tasks',
		loadChildren: () => import('./pages/tasks/tasks.module').then(m => m.TasksModule)
	},
	{
		path: 'users',
		loadChildren: () => import('./pages/users/users.module').then(m => m.UsersModule)
	},
	{
		path: 'admin',
		loadChildren: () => import('./pages/admin/admin.module').then(m => m.AdminModule)
	},
	{
		path: 'reports',
		loadChildren: () => import('./pages/reports/reports.module').then(m => m.ReportsModule)
	},
	{
		path: 'vsts-integration',
		loadChildren: () => import('./pages/vsts-integration/vsts-integration.module').then(m => m.VstsIntegrationModule)
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
