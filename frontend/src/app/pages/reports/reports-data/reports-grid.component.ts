import { Component, Input, OnChanges } from '@angular/core';
import { ArrayUtils } from '../../../core/object-utils';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../../models/user';
import { ReportGridView, ReportItem } from '../../../models/reports';
import { ImpersonationService } from '../../../services/impersonation.service';
import * as moment from 'moment';

export interface ReportGridData {
	gridData: ReportGridView;
	rows: number;
}

@Component({
	selector: 'ct-reports-grid',
	templateUrl: 'reports-grid.component.html'
})

export class ReportsGridComponent implements OnChanges {
	@Input() gridData: ReportGridView;
	@Input() groupById: number;
	@Input() rowsNumber: number;
	@Input() showColumnIds: number[] = [];

	dateFormat: string;
	gridDataRows: ReportItem[] = [];

	private lastEvent: any;

	constructor(private impersonationService: ImpersonationService,
	            private route: ActivatedRoute) {
		this.route.data.forEach((data: { user: User }) => {
			let user = this.impersonationService.impersonationUser || data.user;
			this.dateFormat = user.dateFormat;
		});
	}

	ngOnChanges(changes: any) {
		if (this.gridData) {
			this.loadLazy(this.lastEvent);
		}
	}

	formatDate(utcDate: string): string {
		if (!utcDate) {
			return;
		}
		let date = moment(utcDate);
		return this.dateFormat ? date.format(this.dateFormat) : date.toDate().toLocaleDateString();
	}

	getTimeString(time: number, showDefaultValue: boolean = false): string {
		let m = Math.floor(time / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;

		if (!showDefaultValue && h === 0 && m === 0) {
			return '';
		}

		return (((h > 99) ? ('' + h) : ('00' + h).slice(-2)) + ':' + ('00' + m).slice(-2));
	}

	loadLazy(event: any): void {
		this.lastEvent = event;

		if (event && this.gridData) {
			event.sortField = event.sortField || 'date';
			this.gridData.items = [...ArrayUtils.sortByField(this.gridData.items, event.sortField, event.sortOrder)];

			setTimeout(() => {
				this.gridDataRows = this.gridData.items.slice(0, this.rowsNumber);
			}, 0);
		}
	}
}
