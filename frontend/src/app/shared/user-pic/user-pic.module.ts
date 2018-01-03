import { NgModule } from '@angular/core';
import { UserPicComponent } from './user-pic.component';
import { CommonModule } from '@angular/common';
import { Http } from '@angular/http';
import { CustomHttp } from '../../core/custom-http';

@NgModule({
	imports: [
		CommonModule
	],
	declarations: [
		UserPicComponent
	],
	exports: [
		UserPicComponent
	],
	providers: [
		{
			provide: Http,
			useClass: CustomHttp,
		}
	]
})

export class UserPicModule {
}