import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ODataConfiguration } from '../../../services/odata/config';

export class EmailSendingStatus {
	isSentEmail: boolean;
	message?: number;
}

@Injectable()
export class EnterEmailService {
	constructor(protected config: ODataConfiguration,
	            private http: HttpClient) {
	}

	sendEmail(email): Promise<boolean | EmailSendingStatus> {
		let url = '/api/v1/Password/sendforgotemail/';
		return this.http.get<EmailSendingStatus>(url + email)
			.toPromise()
			.then(res => res)
			.catch((err) => false);
	}
}


