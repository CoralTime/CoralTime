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
import { DirectivesModule } from './directives/directives.module';
import { DatepickerModule } from './form/datepicker/datepicker.module';
import { MenuModule } from './menu/menu.module';
import { SharedFormModule } from './form/shared-form.module';
import { UserPicComponent } from './user-pic/user-pic.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { ExpansionPanelComponent } from './expansion-panel/expansion-panel.component';

@NgModule({
	imports: [
		CommonModule,
		PaginatorModule,
		PrimeNgSharedModule,
		ButtonModule,
		DialogModule,
		DropdownModule,
		CalendarModule,
		DirectivesModule,
		DatepickerModule,
		DataTableModule,
		MenuModule,
		SharedFormModule,
	],
	declarations: [
		ReadMoreComponent,
		ConfirmationComponent,
		ExpansionPanelComponent,
		UserPicComponent
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
		TranslateModule,
		DirectivesModule,
		DatepickerModule,
		DataTableModule,
		MenuModule,
		SharedFormModule,
		UserPicComponent,
		ConfirmationComponent,
		ExpansionPanelComponent
	]
})

export class SharedModule {
}
