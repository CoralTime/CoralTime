import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MultiSelectComponent } from './multiselect/multiselect.component';
import { SelectComponent } from './select/select.component';
import { DirectivesModule } from '../directives/directives.module';
import { ColorPickerComponent } from './color-picker/color-picker.component';
import { TextareaComponent } from './textarea/textarea.component';
import { InputListComponent } from './input-list/input-list.component';
import { TextMaskModule } from 'angular2-text-mask';
import { NgSlimScrollModule } from 'ngx-slimscroll';
import { MaterialModule } from '../material.module';

@NgModule({
	imports: [
		CommonModule,
		DirectivesModule,
		FormsModule,
		MaterialModule,
		NgSlimScrollModule,
		ReactiveFormsModule,
		TextMaskModule
	],
	declarations: [
		ColorPickerComponent,
		InputListComponent,
		MultiSelectComponent,
		SelectComponent,
		TextareaComponent
	],
	exports: [
		FormsModule,
		MaterialModule,
		ReactiveFormsModule,
		ColorPickerComponent,
		InputListComponent,
		MultiSelectComponent,
		SelectComponent,
		TextareaComponent,
		TextMaskModule
	]
})

export class SharedFormModule {
}
