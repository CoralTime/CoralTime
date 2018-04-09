import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
	selector: 'ct-user-pic',
	template: '<img src="{{urlIcon}}">'
})

export class UserPicComponent implements OnInit {
	@Input() fullSize: boolean = false;
	@Input() urlIcon: string;
	@Input() userId: number;
	@HostBinding('class.ct-user-pic-avatar') addClass: boolean = false;

	ngOnInit() {
		this.addClass = this.fullSize;
	}
}
