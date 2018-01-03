import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';

export class PasswordChangingStatus {
	isChangedPassword: boolean;
	message: number;
}

@Injectable()
export class ForgotPasswordService {
	restoreCodeIsExpired: boolean = false;
	private restoreCode: string;

	constructor(private http: Http) {
	}

	saveNewPassword(token: string, password: string): Promise<PasswordChangingStatus> {
		let url = '/api/v1/Password/changepasswordbytoken';
		let body = {
			token: token,
			newPassword: password,
		};
		return this.http.post(url, body)
			.toPromise()
			.then(res => {
				return res.json();
			}).catch((err) => {
				return null;
			});
	}

	validateRestoreCode(restoreCode: string): Observable<any> {
		this.restoreCode = restoreCode;
		let url = '/api/v1/Password/checkforgotpasswordtoken/';
		return this.http.get(url + restoreCode)
			.map(res => {
				let result = res.json();
				this.restoreCodeIsExpired = result['isTokenValid'];
				return this.restoreCodeIsExpired;
			});
	}
}

