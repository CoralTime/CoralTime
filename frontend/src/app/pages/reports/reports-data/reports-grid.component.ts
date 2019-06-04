import { Component, Input, OnChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { ReportGridView, ReportItem } from '../../../models/reports';
import { User } from '../../../models/user';
import { ArrayUtils } from '../../../core/object-utils';
import { ImpersonationService } from '../../../services/impersonation.service';

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
	@Input() isTotalsOnly: boolean;

	gridDataRows: ReportItem[] = [];
	user: User;

	private lastEvent: any;

	constructor(private impersonationService: ImpersonationService,
	            private route: ActivatedRoute) {
		this.route.data.forEach((data: { user: User }) => {
			this.user = this.impersonationService.impersonationUser || data.user;
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
		return this.user.dateFormat ? date.format(this.user.dateFormat) : date.toDate().toLocaleDateString();
	}

	getTimeString(time: number, showDefaultValue: boolean = false, formatToAmPm: boolean = false): string {
		let m = Math.floor(time / 60);
		let h = Math.floor(m / 60);
		m = m - h * 60;

		if (!showDefaultValue && h === 0 && m === 0) {
			return '';
		}

		if (formatToAmPm) {
			let t = new Date().setHours(0, 0, time);
			return moment(t).format('hh:mm A');
		}

		return (((h > 99) ? ('' + h) : ('00' + h).slice(-2)) + ':' + ('00' + m).slice(-2));
	}

	loadLazy(event: any): void {
		this.lastEvent = event;

		console.info('is totals - ',this.isTotalsOnly);
		
		if (event && this.gridData) {
			event.sortField = event.sortField || 'date';
			this.gridData.items = [...ArrayUtils.sortByField(this.gridData.items, event.sortField, event.sortOrder)];

			setTimeout(() => {
				this.gridDataRows = this.gridData.items.slice(0, this.rowsNumber);
			}, 0);
		}
	}
}
