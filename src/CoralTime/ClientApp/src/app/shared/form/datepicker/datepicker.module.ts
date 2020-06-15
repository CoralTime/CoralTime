import { NgModule } from '@angular/core';
import { DpDatePickerModule } from 'ng2-date-picker';
import { DatepickerComponent } from './datepicker.component';
import { FormsModule } from '@angular/forms';

@NgModule({
	imports: [
		FormsModule,
		DpDatePickerModule
	],
	declarations: [
		DatepickerComponent
	],
	exports: [
		DatepickerComponent
	]
})

export class DatepickerModule {
}
