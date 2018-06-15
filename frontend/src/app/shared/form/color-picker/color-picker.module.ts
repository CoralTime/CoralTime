import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ColorPickerModule as ColorModule } from 'primeng/primeng';
import { ColorPickerComponent } from './color-picker.component';
import { TextMaskModule } from 'angular2-text-mask';

@NgModule({
	imports: [
		FormsModule,
		ColorModule,
		TextMaskModule
	],
	declarations: [
		ColorPickerComponent
	],
	exports: [
		ColorPickerComponent
	]
})

export class ColorPickerModule {
}
