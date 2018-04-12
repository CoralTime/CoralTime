import { Component, EventEmitter, Output, Input } from '@angular/core';
import { DateUtils } from '../../../models/calendar';
import { DateStatic } from '../../../models/reports';
import { DatePeriod, DateResponse } from './range-datepicker.service';
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-range-datepicker',
	templateUrl: 'range-datepicker.component.html',
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	}
})

export class RangeDatepickerComponent {
	@Input() firstDayOfWeek: number;
	@Input() datePeriodList: DateStatic[];

	@Input('dateResponse')
	set assignedDatePeriod(dateResponse: DateResponse) {
		this.oldDateResponse = dateResponse;
		this.dateFrom = DateUtils.convertMomentToUTCMoment(dateResponse.datePeriod.dateFrom);
		this.dateTo = DateUtils.convertMomentToUTCMoment(dateResponse.datePeriod.dateTo);
		this.displayDate = this.dateFrom;
		this.selectedRange = this.getRangeBetweenDates(this.dateFrom.toDate(), this.dateTo.toDate());
	}

	@Output() closed: EventEmitter<void> = new EventEmitter<void>();
	@Output() onPeriodChanged: EventEmitter<DateResponse> = new EventEmitter();

	displayDate: Moment;
	selectedRange: Moment[] = [];

	private clickedDay: Moment;
	private dateFrom: Moment;
	private dateTo: Moment;
	private daySelectedNumber: number = 0;
	private oldDateResponse: DateResponse;

	dateOnClick(day: Moment): void {
		this.clickedDay = day;
		this.daySelectedNumber++;

		switch (this.daySelectedNumber) {
			case 1: {
				this.dateFrom = this.clickedDay;
				this.selectedRange = [this.clickedDay];
				break;
			}
			case 2: {
				if (this.clickedDay.isAfter(this.dateFrom)) {
					this.dateTo = this.clickedDay;
				} else {
					this.dateTo = this.dateFrom;
					this.dateFrom = this.clickedDay;
				}
				this.selectedRange = this.getRangeBetweenDates(this.dateFrom.toDate(), this.dateTo.toDate());
				break;
			}
			default: {
				this.daySelectedNumber = 1;
				this.dateTo = null;
				this.dateFrom = this.clickedDay;
				this.selectedRange = [this.clickedDay];
			}
		}

		this.displayDate = this.clickedDay;
		this.onPeriodChanged.emit({
			datePeriod: new DatePeriod(this.dateFrom, this.dateTo),
			dateStaticId: null
		});
	}

	setPeriod(period: DateStatic): void {
		this.selectedRange = this.getRangeBetweenDates(new Date(period.dateFrom), new Date(period.dateTo));
		this.onPeriodChanged.emit({
			datePeriod: new DatePeriod(moment(period.dateFrom), moment(period.dateTo)),
			dateStaticId: period.id
		});
		this.closed.emit();
	}

	onKeyDown(event: KeyboardEvent): void {
		if (event.key === 'Escape') {
			this.onPeriodChanged.emit(this.oldDateResponse);
			this.closed.emit();
		}
	}

	private getRangeBetweenDates(dateFrom: Date, dateTo: Date): Moment[] {
		let listDate = [];
		let dateMove = dateFrom;
		let strDate = dateFrom;

		while (strDate.getTime() <= dateTo.getTime()) {
			strDate = dateMove;
			listDate.push(moment(strDate));
			dateMove.setDate(dateMove.getDate() + 1);
		}

		return listDate;
	}
}
