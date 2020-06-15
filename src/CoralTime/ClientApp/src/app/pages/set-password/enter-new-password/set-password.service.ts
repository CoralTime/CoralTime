import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export class PasswordChangingStatus {
	isChangedPassword: boolean;
	message: number;
}

@Injectable()
export class SetPasswordService {
	restoreCodeIsExpired: boolean = false;
	private restoreCode: string;

	constructor(private http: HttpClient) {
	}

	saveNewPassword(token: string, password: string): Promise<PasswordChangingStatus> {
		let url = '/api/v1/Password/changepasswordbytoken';
		let body = {
			token: token,
			newPassword: password,
		};
		return this.http.post(url, body)
			.toPromise()
			.then(res => res)
			.catch(() => null);
	}

	validateRestoreCode(restoreCode: string): Observable<any> {
		this.restoreCode = restoreCode;
		let url = '/api/v1/Password/checkforgotpasswordtoken/';

		return this.http.get(url + restoreCode)
			.map(res => {
				this.restoreCodeIsExpired = res['isTokenValid'];
				return this.restoreCodeIsExpired;
			});
	}
}

