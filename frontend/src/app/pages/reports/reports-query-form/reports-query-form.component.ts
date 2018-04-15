import { Component, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ReportsService } from '../../../services/reposts.service';
import { ReportFilters } from '../../../models/reports';

@Component({
	selector: 'ct-reports-query-form',
	templateUrl: 'reports-query-form.component.html'
})

export class ReportsQueryFormComponent {
	isRequestLoading: boolean = false;
	model: ReportFilters;

	@Output() onSubmit = new EventEmitter();

	constructor(private reportsService: ReportsService) {
	}

	submit(form: NgForm): void {
		if (form.invalid) {
			return;
		}

		this.isRequestLoading = true;
		this.reportsService.createQuery({
			currentQuery: this.model
		}).subscribe(() => {
				this.isRequestLoading = false;
				this.onSubmit.emit(null);
			},
			error => this.onSubmit.emit(error)
		);
	}
}
