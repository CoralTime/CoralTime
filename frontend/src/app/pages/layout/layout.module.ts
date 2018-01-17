import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { MenubarModule } from 'primeng/primeng';
import { MenuModule } from 'primeng/primeng';
import { ButtonModule } from 'primeng/primeng';
import { SharedModule } from '../../shared/shared.module';
import { NavigationComponent } from './navigation/navigation.component';

@NgModule({
	imports: [
		MenubarModule,
		ButtonModule,
		MenuModule,
		RouterModule,
		SharedModule
	],
	exports: [
		NavigationComponent
	],
	declarations: [
		NavigationComponent
	]
})

export class LayoutModule {
}
