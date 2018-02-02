import { Component, Output, EventEmitter } from '@angular/core';
import { ReportFilters, ReportsService } from '../../../services/reposts.service';
import { NgForm } from '@angular/forms';

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
			valuesSaved: this.model
		}).subscribe(() => {
				this.isRequestLoading = false;
				this.onSubmit.emit(null);
			},
			error => this.onSubmit.emit(error)
		);
	}
}
