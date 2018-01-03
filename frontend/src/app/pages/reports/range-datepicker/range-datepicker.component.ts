import { Component, EventEmitter, Output, Input, AfterContentInit } from '@angular/core';
import { DateUtils } from '../../../models/calendar';
import { DatePeriod, RangeDatepickerService } from './range-datepicker.service';
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-range-datepicker',
	templateUrl: 'range-datepicker.component.html',
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	}
})

export class RangeDatepickerComponent implements AfterContentInit {
	@Input() firstDayOfWeek: number;

	@Input('datePeriod')
	set assignedDatePeriod(datePeriod: DatePeriod) {
		this.dateFrom = datePeriod.dateFrom;
		this.dateTo = datePeriod.dateTo;
	}

	@Output() closed: EventEmitter<void> = new EventEmitter<void>();
	@Output() onPeriodChanged: EventEmitter<DatePeriod> = new EventEmitter();

	datePeriod: DatePeriod;
	displayDate: Moment;
	selectedRange: Moment[] = [];
	DATE_PERIOD = this.rangeDatepickerService.getDatePeriodList();

	private clickedDay: Moment;
	private dateFrom: Moment;
	private dateTo: Moment;
	private daySelectedNumber: number = 0;
	private oldDatePeriod: DatePeriod;

	constructor(private rangeDatepickerService: RangeDatepickerService) {
	}

	ngAfterContentInit() {
		if (this.dateFrom || this.dateTo) {
			this.daySelectedNumber = 2;
			this.dateFrom = DateUtils.convertMomentToUTCMoment(this.dateFrom);
			this.dateTo = this.dateTo ? DateUtils.convertMomentToUTCMoment(this.dateTo) : DateUtils.convertMomentToUTCMoment(this.dateFrom);
			this.displayDate = this.dateFrom;
			this.selectedRange = this.getRangeBetweenDates(this.dateFrom.toDate(), this.dateTo.toDate());
		}
		this.oldDatePeriod = new DatePeriod(this.dateFrom, this.dateTo);
	}

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

		this.datePeriod = new DatePeriod(this.dateFrom, this.dateTo);
		this.displayDate = this.clickedDay;
		this.onPeriodChanged.emit(this.datePeriod);
	}

	setPeriod(period: DatePeriod): void {
		this.selectedRange = this.getRangeBetweenDates(period.dateFrom.toDate(), period.dateTo.toDate());
		this.onPeriodChanged.emit(period);
		this.closed.emit();
	}

	onKeyDown(event: KeyboardEvent): void {
		if (event.key == 'Escape') {
			this.onPeriodChanged.emit(this.oldDatePeriod);
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
