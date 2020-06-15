import { NgModule } from '@angular/core';
import { EntryTimeFormComponent } from './entry-time-form/entry-time-form.component';
import { EntryTimeComponent } from './entry-time.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../../shared/shared.module';
import { MultipleDatepickerComponent } from './multiple-datepicker/multiple-datepicker.component';

@NgModule({
	imports: [
		FormsModule,
		SharedModule
	],
	declarations: [
		EntryTimeComponent,
		EntryTimeFormComponent,
		MultipleDatepickerComponent
	],
	entryComponents: [
		MultipleDatepickerComponent
	],
	exports: [
		EntryTimeComponent
	]
})

export class EntryTimeModule {
}
