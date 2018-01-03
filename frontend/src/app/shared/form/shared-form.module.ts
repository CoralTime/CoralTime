import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MultiSelect } from './multiselect/multiselect.component';
import { SelectComponent } from './select/select.component';
import { MaterialModule } from '@angular/material';
import { DirectivesModule } from '../directives/directives.module';
import { SlimScrollModule } from 'ng2-slimscroll';
import { ColorPickerComponent } from './color-picker/color-picker.component';
import { TextareaComponent } from './textarea/textarea.component';
import { InputListComponent } from './input-list/input-list.component';

@NgModule({
	imports: [
		CommonModule,
		FormsModule,
		ReactiveFormsModule,
		MaterialModule,
		DirectivesModule,
		SlimScrollModule
	],
	declarations: [
		ColorPickerComponent,
		MultiSelect,
		SelectComponent,
		TextareaComponent,
		InputListComponent
	],
	exports: [
		FormsModule,
		ReactiveFormsModule,
		MaterialModule,
		ColorPickerComponent,
		MultiSelect,
		SelectComponent,
		TextareaComponent,
		InputListComponent
	]
})

export class SharedFormModule {
}
