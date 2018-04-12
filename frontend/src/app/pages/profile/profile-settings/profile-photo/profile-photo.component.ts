import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { Bounds, CropperSettings, ImageCropperComponent } from 'ng2-img-cropper';
import { NotificationService } from '../../../../core/notification.service';
import { MatDialog } from '@angular/material';
import { UserPicService } from '../../../../services/user-pic.service';

@Component({
	selector: 'ct-profile-photo-dialog',
	templateUrl: 'profile-photo.component.html'
})

export class ProfilePhotoComponent {
	cropperSettings: CropperSettings;
	imageData: any;
	fileName: string;
	fileType: string;

	@Output() onSubmit = new EventEmitter();
	@ViewChild(ImageCropperComponent) cropper: ImageCropperComponent;

	constructor(private notificationService: NotificationService,
	            private matDialog: MatDialog,
	            private userPicService: UserPicService) {
		this.cropperSettings = new CropperSettings();
		this.cropperSettings.width = 200;
		this.cropperSettings.height = 200;

		this.cropperSettings.croppedWidth = 200;
		this.cropperSettings.croppedHeight = 200;

		this.cropperSettings.canvasWidth = 300;
		this.cropperSettings.canvasHeight = 300;

		this.cropperSettings.minWidth = 10;
		this.cropperSettings.minHeight = 10;

		this.cropperSettings.rounded = false;
		this.cropperSettings.keepAspect = true;

		this.cropperSettings.cropperDrawSettings.strokeColor = 'rgba(255,255,255,1)';
		this.cropperSettings.cropperDrawSettings.strokeWidth = 2;

		this.imageData = {};
	}

	cropped(bounds: Bounds): void {
		this.cropperSettings.croppedHeight = 400;
		this.cropperSettings.croppedWidth = 400;
	}

	changeProfileImg(base64String: string): void {
		this.userPicService.uploadUserPicture(this.createCroppedImg(base64String))
			.subscribe((avatar: string) => {
					let avatarUrl = avatar;
					this.notificationService.success('Your profile photo was changed.');
					this.onSubmit.emit(avatarUrl);
				},
				error => {
					this.notificationService.danger('Error changing profile photo.');
					this.matDialog.closeAll();
				});
	}

	fileChangeListener(event): void {
		let image: any = new Image();
		let file: File = event.target.files[0];
		let myReader: FileReader = new FileReader();
		let that = this;
		that.fileName = file.name;
		that.fileType = file.type;

		myReader.onloadend = function (loadEvent: any) {
			image.src = loadEvent.target.result;
			that.cropper.setImage(image);
		};

		myReader.readAsDataURL(file);
	}

	private base64ToBlob(base64String: string): Blob {
		let binary = atob(this.sliceCoding(base64String));
		let len = binary.length;
		let buffer = new ArrayBuffer(len);
		let view = new Uint8Array(buffer);
		for (let i = 0; i < len; i++) {
			view[i] = binary.charCodeAt(i);
		}

		return new Blob([view], {type: this.fileType});
	};

	private blobToFile(theBlob: Blob, fileName: string): File {
		let b: any = theBlob;
		b.lastModifiedDate = new Date();
		b.name = fileName;

		return <File>b;
	}

	private createCroppedImg(base64String: string): File {
		let imgBlob: Blob = this.base64ToBlob(base64String);
		return this.blobToFile(imgBlob, this.fileName);
	}

	private sliceCoding(base64String: string): string {
		return base64String.slice(base64String.indexOf('base64,') + 7);
	}
}
