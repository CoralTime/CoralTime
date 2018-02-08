import { DataTableModule } from './datatable/datatable';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
	PaginatorModule,
	SharedModule as PrimeNgSharedModule,
	ButtonModule,
	DialogModule,
	DropdownModule,
	CalendarModule
} from 'primeng/primeng';
import { ReadMoreComponent } from './read-more/read-more.component';
import { TranslateModule } from '@ngx-translate/core';
import { SlimScrollModule } from 'ng2-slimscroll';
import { DirectivesModule } from './directives/directives.module';
import { LoadingBarComponent } from './loading-indicator/loading-bar.component';
import { DatepickerModule } from './form/datepicker/datepicker.module';
import { MenuModule } from './menu/menu.module';
import { SharedFormModule } from './form/shared-form.module';
import { element } from 'protractor';
import { UserPicModule } from './user-pic/user-pic.module';
import { ConfirmationComponent } from './confirmation/confirmation.component';

@NgModule({
	imports: [
		CommonModule,
		PaginatorModule,
		PrimeNgSharedModule,
		ButtonModule,
		DialogModule,
		DropdownModule,
		CalendarModule,
		SlimScrollModule,
		DirectivesModule,
		DatepickerModule,
		DataTableModule,
		MenuModule,
		SharedFormModule,
		UserPicModule
	],
	declarations: [
		ReadMoreComponent,
		LoadingBarComponent,
		ConfirmationComponent
	],
	exports: [
		CommonModule,
		PaginatorModule,
		PrimeNgSharedModule,
		ButtonModule,
		DialogModule,
		DropdownModule,
		CalendarModule,
		ReadMoreComponent,
		TranslateModule,
		LoadingBarComponent,
		TranslateModule,
		SlimScrollModule,
		DirectivesModule,
		DatepickerModule,
		DataTableModule,
		MenuModule,
		SharedFormModule,
		UserPicModule,
		ConfirmationComponent
	]
})

export class SharedModule {
}
