import { Component } from '@angular/core';

@Component({
	selector: 'ct-about',
	templateUrl: 'about.component.html'
})

export class AboutComponent {
	message: string;

	constructor() {
		this.message = 'Coral TimeTracker v.0.1';
	}
}
