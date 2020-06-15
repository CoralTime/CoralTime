import { Component, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ReportQuery } from '../../../models/reports';
import { ReportsService } from '../../../services/reposts.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';

@Component({
	selector: 'ct-reports-query-form',
	templateUrl: 'reports-query-form.component.html'
})

export class ReportsQueryFormComponent {
	isRequestLoading: boolean = false;
	model: ReportQuery;

	@Output() onSubmit = new EventEmitter();

	constructor(private loadingService: LoadingMaskService,
	            private reportsService: ReportsService) {
	}

	submit(form: NgForm): void {
		if (form.invalid) {
			return;
		}

		this.isRequestLoading = true;
		this.loadingService.addLoading();
		this.reportsService.createQuery({
			currentQuery: this.model
		})
			.finally(() => this.loadingService.removeLoading())
			.subscribe(() => {
					this.isRequestLoading = false;
					this.onSubmit.emit(null);
				},
				error => this.onSubmit.emit(error)
			);
	}
}
