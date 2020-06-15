import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile.component';
import { SharedModule } from '../../shared/shared.module';
import { ProfileSettingsComponent } from './profile-settings/profile-settings.component';
import { ProfileService } from '../../services/profile.service';
import { EnterEmailService } from '../set-password/enter-email/enter-email.service';
import { ProfilePhotoComponent } from './profile-settings/profile-photo/profile-photo.component';
import { ImageCropperModule } from 'ng2-img-cropper';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
	imports: [
		CommonModule,
		ProfileRoutingModule,
		FileUploadModule,
		ImageCropperModule,
		SharedModule
	],
	declarations: [
		ProfileComponent,
		ProfileSettingsComponent,
		ProfilePhotoComponent
	],
	providers: [
		ProfileService,
		EnterEmailService
	],
	entryComponents: [
		ProfilePhotoComponent
	]
})

export class ProfileModule {
}
