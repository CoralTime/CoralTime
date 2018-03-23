import { Response } from '@angular/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ConstantService } from '../core/constant.service';
import { CustomHttp } from '../core/custom-http';

@Injectable()
export class UserPicService {
	constructor(private http: CustomHttp,
	            private constantService: ConstantService) {
	}

	loadUserPicture(userId: number): Observable<string> {
		return this.http.get(this.constantService.profileApi + '/Member(' + userId + ')/UrlAvatar')
			.map((res: Response) => res.text());
	}

	uploadUserPicture(fileToUpload: File): Observable<string> {
		let input = new FormData();
		input.append('file', fileToUpload, fileToUpload.name);

		return this.http.put(this.constantService.profileApi + '/UploadImage', input)
			.map((res: Response) => res.text());
	}
}
