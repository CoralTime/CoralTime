import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { ReportDropdowns, ReportsService } from '../../services/reposts.service';

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
