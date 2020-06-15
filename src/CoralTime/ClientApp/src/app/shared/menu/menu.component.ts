import { Component, ContentChildren, QueryList, AfterContentInit, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { MenuItemComponent } from './menu-item/menu-item.component';

@Component({
	selector: 'ct-menu',
	templateUrl: 'menu.component.html'
})

export class MenuComponent implements AfterContentInit {
	@Input() autoClose: boolean = true;

	_xPosition: string = 'after';
	@Input()
	get xPosition() { return this._xPosition; }

	set xPosition(value: string) {
		if (value !== 'before' && value !== 'after') {
			return;
		}
		this._xPosition = value;
	}

	@ContentChildren(MenuItemComponent) items: QueryList<MenuItemComponent>;
	@Output() closed: EventEmitter<void> = new EventEmitter<void>();

	canClose: boolean = true;
	isOpen: boolean = false;

	ngAfterContentInit() {
		this.items.toArray().forEach((item: MenuItemComponent) => {
			item.autoClose = item.autoClose == null ? this.autoClose : item.autoClose;
			item.closed.subscribe(() => {
				this.closeMenu();
			});
		});
	}

	openMenu(): void {
		if (this.canClose) {
			this.changeCloseParameter();
			this.isOpen = true;
		}
	}

	closeMenu(): void {
		if (this.canClose) {
			this.changeCloseParameter();
			this.isOpen = false;
			this.closed.emit();
		}
	}

	toggleMenu(): void {
		if (this.isOpen) {
			this.closeMenu();
		} else {
			this.openMenu();
		}
	}

	private changeCloseParameter(): void {
		this.canClose = false;
		setTimeout(() => this.canClose = true, 300);
	}
}
