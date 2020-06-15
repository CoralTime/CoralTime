import { NgModule } from '@angular/core';
import { MenuComponent } from './menu.component';
import { MenuItemComponent } from './menu-item/menu-item.component';
import { DirectivesModule } from '../directives/directives.module';

@NgModule({
	imports: [
		DirectivesModule
	],
	declarations: [
		MenuComponent,
		MenuItemComponent
	],
	exports: [
		MenuItemComponent,
		MenuComponent
	]
})

export class MenuModule {
}
