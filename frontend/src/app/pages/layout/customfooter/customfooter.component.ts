import { Component } from '@angular/core';

@Component({
	selector: 'customfooter',
	templateUrl: 'customfooter.component.html'
})

export class CustomfooterComponent {
	constructor() {
	}

	currentYear: number = new Date().getFullYear();
}