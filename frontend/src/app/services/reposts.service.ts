import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { saveAs as importedSaveAs } from 'file-saver';
import { ConstantService } from '../core/constant.service';
import { SendReportsFormModel } from '../pages/reports/reports-send/reports-send.component';
import { ReportDropdowns, ReportGrid } from '../models/reports';

@Injectable()
export class ReportsService {
	constructor(private constantService: ConstantService,
	            private http: HttpClient) {
	}

	createQuery(obj: any): Observable<any> {
		return this.http.post(this.constantService.reportsApi + 'Settings/CustomQuery', obj);
	}

	deleteQuery(queryId: number): Observable<any> {
		return this.http.delete(this.constantService.reportsApi + 'Settings/CustomQuery/' + queryId);
	}

	getReportDropdowns(): Observable<ReportDropdowns> {
		return this.http.get(this.constantService.reportsApi).map((res) => <ReportDropdowns>res);
	}

	getReportGrid(filters: any): Observable<ReportGrid> {
		return this.http.post(this.constantService.reportsApi, filters).map((res) => <ReportGrid>res);
	}

	exportAs(filters: any): Observable<void> {
		let options = {
			observe: 'response' as 'response',
			responseType: 'blob' as 'blob'
		};

		return this.http.post(this.constantService.reportsApi + 'ExportFile', filters, options)
			.map((data: HttpResponse<Blob>) => {
				let filename = 'reports';

				if (data.headers.has('Content-Disposition')) {
					let cdHeader = data.headers.get('Content-Disposition');
					filename = cdHeader.replace(/(.*filename=")(.*)(";.*)/, '$2');
				}

				importedSaveAs(data.body, filename);
			});
	}

	sendReports(filters: SendReportsFormModel): Observable<any> {
		return this.http.post(this.constantService.reportsApi + 'ExportEmail', filters, {observe: 'response'});
	}
}
