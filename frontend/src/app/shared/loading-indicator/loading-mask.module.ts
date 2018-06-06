import { NgModule } from '@angular/core';
import { LoadingMaskService } from './loading-mask.service';
import { LoadingMaskComponent } from './loading-mask.component';
import { CommonModule } from '@angular/common';

@NgModule({
	imports: [CommonModule],
	declarations: [LoadingMaskComponent],
	exports: [LoadingMaskComponent],
	providers: [LoadingMaskService]
})

export class LoadingMaskModule {
}
