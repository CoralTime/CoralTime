import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { ODataConfiguration } from '../../../services/odata/config';

export class EmailSendingStatus {
	isSentEmail: boolean;
	message?: number;
}

@Injectable()
export class EnterEmailService {
	constructor(private http: Http,
	            protected config: ODataConfiguration) {
	}

	sendEmail(email): Promise<EmailSendingStatus> {
		let url = '/api/v1/Password/sendforgotemail/';
		return this.http.get(url + email)
			.toPromise()
			.then(res => {
				return res.json();
			})
			.catch((err) => {
				return false;
			});
	}
}


