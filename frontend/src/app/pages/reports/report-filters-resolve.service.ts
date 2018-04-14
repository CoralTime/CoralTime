import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { ReportsService } from '../../services/reposts.service';
import { ReportDropdowns } from '../../models/reports';

@Injectable()
export class ReportFiltersResolveService implements Resolve<ReportDropdowns> {
	constructor(private reportsService: ReportsService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<ReportDropdowns> {
		return this.reportsService.getReportDropdowns()
			.toPromise()
			.then((reportDropdowns: ReportDropdowns) => {
				return reportDropdowns;
			});
	}
}
