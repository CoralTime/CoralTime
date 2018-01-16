import { Component, Input, OnChanges } from '@angular/core';
import { ReportGridView, ReportItem } from '../../../services/reposts.service';
import { ArrayUtils } from '../../../core/object-utils';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../../models/user';
import { ImpersonationService } from '../../../services/impersonation.service';
import * as moment from 'moment';

@Component({
	selector: 'ct-reports-grid',
	templateUrl: 'reports-grid.component.html'
})

export class ReportsGridComponent implements OnChanges {
	@Input() gridData: ReportGridView;
	@Input() groupBy: number;
	@Input() showColumns: number[];

	dateFormat: string;
	gridDataRows: ReportItem[];

	constructor(private impersonationService: ImpersonationService,
	            private route: ActivatedRoute) {
		this.route.data.forEach((data: { user: User }) => {
			let user = this.impersonationService.impersonationUser || data.user;
			this.dateFormat = user.dateFormat;
		});
	}

	ngOnChanges(changes: any) {
		if (this.gridData) {
			this.gridDataRows = this.gridData.items;
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

	loadLazy(event): void {
		event.sortField = event.sortField || 'date';
		if (event && this.gridData) {
			this.gridDataRows = [...ArrayUtils.sortByField(this.gridData.items, event.sortField, event.sortOrder)];
		}
	}
}
