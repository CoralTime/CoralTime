import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
	selector: 'ct-user-pic',
	template: '<img *ngIf="urlIcon" src="{{urlIcon}}">'
})

export class UserPicComponent implements OnInit {
	@Input() fullSize: boolean = false;
	@Input() urlIcon: string;
	@HostBinding('class.ct-user-pic-avatar') addClass: boolean = false;

	ngOnInit() {
		this.addClass = this.fullSize;
	}
}
