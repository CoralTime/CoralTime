import { Component, Output, EventEmitter, HostListener, Input } from '@angular/core';

@Component({
	selector: 'ct-menu-item',
	templateUrl: 'menu-item.component.html'
})

export class MenuItemComponent {
	@Input() autoClose: boolean;
	@Output() closed: EventEmitter<void> = new EventEmitter<void>();

	@HostListener('click', ['$event'])
	elementClicked() {
		if (this.autoClose) {
			this.closed.emit();
		}
	}
}
