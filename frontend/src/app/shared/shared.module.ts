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
import { DirectivesModule } from './directives/directives.module';
import { DatepickerModule } from './form/datepicker/datepicker.module';
import { MenuModule } from './menu/menu.module';
import { SharedFormModule } from './form/shared-form.module';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { ChartComponent } from './chart/chart.component';
import { TranslateModule } from '@ngx-translate/core';
import { MarkdownModule, MarkedOptions } from 'ngx-markdown';

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
		MarkdownModule.forRoot({
			markedOptions: {
				provide: MarkedOptions,
				useValue: {
					breaks: true,
					headerIds: false,
					pedantic: true,
					sanitize: true,
				},
			},
		}),
	],
	declarations: [
		ReadMoreComponent,
		ConfirmationComponent,
		ChartComponent,
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
		DirectivesModule,
		DatepickerModule,
		DataTableModule,
		MenuModule,
		SharedFormModule,
		ConfirmationComponent,
		ChartComponent,
		TranslateModule,
		MarkdownModule,
	]
})

export class SharedModule {
}
