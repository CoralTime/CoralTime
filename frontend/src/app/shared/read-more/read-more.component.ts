import { Component, Input, OnChanges } from '@angular/core';

@Component({
	selector: 'ct-read-more',
	template: `
        <div *ngIf="text">
            <div class="ct-screen-mode" markdown [data]="currentText"></div>
            <div class="ct-print-mode" markdown [data]="text"></div>
            <span *ngIf='showButton' (click)="showText()" class="ct-see-more">{{linkText}}</span>
        </div>
	`
})

export class ReadMoreComponent implements OnChanges {
	@Input('text') text: string;
	@Input() maxLength: number = 300;

	currentText: string;
	isCollapsed: boolean = true;
	linkText: string = 'More';
	showButton: boolean = true;

	showText(): void {
		this.linkText = 'Less';
		this.isCollapsed = !this.isCollapsed;
		this.determineView();
	}

	determineView(): void {
		if (!this.text) {
			this.showButton = false;
			return;
		}
		if (this.text.length <= this.maxLength) {
			this.currentText = this.text;
			this.isCollapsed = false;
			this.showButton = false;
			return;
		}
		if (this.isCollapsed === true) {
			this.showButton = true;
			this.linkText = 'More';
			this.currentText = this.text.substring(0, this.maxLength) + '...';
		} else if (this.isCollapsed === false) {
			this.currentText = this.text;
			this.linkText = 'Less';
		}

	}

	ngOnChanges() {
		this.determineView();
	}
}
