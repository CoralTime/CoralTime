import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
	selector: 'ct-user-pic',
	template: '<img [src]="iconUrl">'
})

export class UserPicComponent implements OnInit {
	@Input() userId: number;
	@Input() iconUrl: string;
	@Input() fullSize: boolean = false;
	@HostBinding('class.ct-user-pic-avatar') addClass: boolean = false;

	ngOnInit() {
		this.addClass = this.fullSize;
	}
}
