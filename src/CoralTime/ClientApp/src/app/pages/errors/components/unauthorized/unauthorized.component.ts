import { Component, OnInit } from '@angular/core';
import { Message } from 'primeng/primeng';

@Component({
	selector: 'ct-unauthorized',
	templateUrl: 'unauthorized.component.html'
})

export class UnauthorizedComponent implements OnInit {
	message: string;
	msgs: Message[] = [];

	constructor() {
		this.message = '401: Unauthorized ';
	}

	ngOnInit() {
		this.showError();
	}

	showError(): void {
		this.msgs = [];
		this.msgs.push({severity: 'error', summary: 'Error ', detail: this.message});
	}
}
