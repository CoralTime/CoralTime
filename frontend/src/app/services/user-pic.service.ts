import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConstantService } from '../core/constant.service';

@Injectable()
export class UserPicService {
	constructor(private http: HttpClient,
	            private constantService: ConstantService) {
	}

	loadUserPicture(userId: number): Observable<string> {
		return this.http.get(
			this.constantService.profileApi + '/Member(' + userId + ')/UrlAvatar',
			{responseType: 'text'}
		);
	}

	uploadUserPicture(fileToUpload: File): Observable<string> {
		let input = new FormData();
		input.append('file', fileToUpload, fileToUpload.name);

		return this.http.put(this.constantService.profileApi + '/UploadImage', input, {responseType: 'text'});
	}
}
