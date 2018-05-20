import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { ReportsService } from '../../services/reposts.service';
import { ReportDropdowns } from '../../models/reports';
import { LoadingMaskService } from '../../shared/loading-indicator/loading-mask.service';

@Injectable()
export class ReportFiltersResolveService implements Resolve<ReportDropdowns> {
	constructor(private loadingService: LoadingMaskService,
	            private reportsService: ReportsService) {
	}

	resolve(route: ActivatedRouteSnapshot): Promise<ReportDropdowns> {
		this.loadingService.addLoading();
		return this.reportsService.getReportDropdowns()
			.toPromise()
			.then((reportDropdowns: ReportDropdowns) => {
				this.loadingService.removeLoading();
				return reportDropdowns;
			});
	}
}
