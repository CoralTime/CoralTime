import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';

const routes: Routes = [
	{path: 'forbidden', component: ForbiddenComponent},
	{path: 'unauthorized', component: UnauthorizedComponent}
];

@NgModule({
	imports: [RouterModule.forChild(routes)],
	exports: [RouterModule],
	providers: []
})

export class ErrorsRoutingModule {
}
