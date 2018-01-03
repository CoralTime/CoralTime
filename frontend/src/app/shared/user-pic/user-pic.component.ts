import { Component, HostBinding, Input, OnChanges } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { UserPicService } from '../../services/user-pic.service';

const IMG_BASE64 = 'data:image/jpg;base64,';

@Component({
	selector: 'ct-user-pic',
	templateUrl: 'user-pic.component.html'
})

export class UserPicComponent implements OnChanges {
	@Input() userId: number;
	@Input() fullSize: boolean = false;
	@HostBinding('class.ct-user-pic-avatar') addClass: boolean = false;

	imageData: string;
	hasUserPic: boolean = false;

	private subscriptionProfilePhoto: Subscription;

	constructor(private userPicService: UserPicService) {
	}

	ngOnInit() {
		this.addClass = this.fullSize;
		this.subscriptionProfilePhoto = this.userPicService.onUserPicChange.subscribe(() => {
			this.getUserPicture(this.userId, this.fullSize);
		});
	}

	ngOnChanges(changes: any) {
		this.getUserPicture(this.userId, this.fullSize);
	}

	getUserPicture(userId: number, fullSize: boolean): void {
		this.userPicService.getUserPicture(userId, fullSize).subscribe((img: string) => {
			if (img.length) {
				this.imageData = IMG_BASE64 + img;
				this.hasUserPic = true;
			} else {
				this.hasUserPic = false;
			}
		})
	};
}