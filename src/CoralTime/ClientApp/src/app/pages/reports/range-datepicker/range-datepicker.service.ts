import { Injectable } from '@angular/core';
import { DateStatic } from '../../../models/reports';
import * as moment from 'moment';
import Moment = moment.Moment;

export class DatePeriod {
	dateFrom: Moment;
	dateTo: Moment;

	constructor(dateFrom: Moment, dateTo?: Moment) {
		this.dateFrom = dateFrom;
		this.dateTo = dateTo || dateFrom;
	}
}

export interface DateResponse {
	datePeriod: DatePeriod;
	dateStaticId: number
}

@Injectable()
export class RangeDatepickerService {
	dateStaticList: DateStatic[];

	setDateStringPeriod(period: DatePeriod): string {
		for (let dateStatic of this.dateStaticList) {
			if (this.isDatePeriodEqual(period, dateStatic)) {
				return dateStatic.description;
			}
		}

		if (this.isFromPeriod(period, 1)) {
			let weekDayFrom = period.dateFrom.toDate().toLocaleString('en-us', {weekday: 'short'});
			let weekDayTo = period.dateTo.toDate().toLocaleString('en-us', {weekday: 'short'});

			return this.isSingleDay(period) ? weekDayFrom : weekDayFrom + ' - ' + weekDayTo;
		}

		let dateString: string;
		let monthFormat = 'long';
		let monthNameFrom = period.dateFrom.toDate().toLocaleString('en-us', {month: monthFormat});
		let yearFrom = period.dateFrom.year();

		if (this.isFromOneMonth(period)) {
			dateString = monthNameFrom + ' ' + this.uniteDays(period);
			return this.isFromPeriod(period, 3) ? dateString : dateString + ' ' + yearFrom;
		}

		monthFormat = this.isIntegerNumberOfMonths(period) ? 'long' : 'short';
		let monthDayFrom = period.dateFrom.toDate().getDate();
		let monthDayTo = period.dateTo.toDate().getDate();
		monthNameFrom = period.dateFrom.toDate().toLocaleString('en-us', {month: monthFormat});
		let monthNameTo = period.dateTo.toDate().toLocaleString('en-us', {month: monthFormat});

		if (this.isFromOneYear(period)) {
			dateString = monthNameFrom + ' ' + monthDayFrom + ' - ' + monthNameTo + ' ' + monthDayTo;
			return this.isFromPeriod(period, 3) ? dateString : dateString + ', ' + period.dateFrom.year();
		}

		let yearTo = period.dateTo.year();

		return monthNameFrom + ' ' + monthDayFrom + ', ' + yearFrom + ' - ' + monthNameTo + ' ' + monthDayTo + ', ' + yearTo;
	}

	isDatePeriodEqual(a: DatePeriod, b: DateStatic): boolean {
		return a.dateFrom.toDate().toDateString() === new Date(b.dateFrom).toDateString()
			&& a.dateTo.toDate().toDateString() === new Date(b.dateTo).toDateString()
	}

	isIntegerNumberOfMonths(period: DatePeriod): boolean {
		let nextDay = (d => new Date(d.setDate(d.getDate() + 1)))(period.dateTo.toDate());
		return period.dateFrom.date() === 1 && nextDay.getDate() === 1;
	}

	private isFromPeriod(period: DatePeriod, periodId: number): boolean {
		return period.dateFrom.toDate().getTime() >= new Date(this.dateStaticList[periodId].dateFrom).getTime() &&
			period.dateTo.toDate().getTime() <= new Date(this.dateStaticList[periodId].dateTo).getTime();
	}

	private isFromOneMonth(period: DatePeriod): boolean {
		let d: Date = period.dateFrom.toDate();
		let monthBeginDay: Date = new Date(d.getFullYear(), d.getMonth(), 1);
		let monthEndDay: Date = new Date(d.getFullYear(), d.getMonth() + 1, 1, 0, 0, 0, -1);

		return period.dateFrom.toDate().getTime() >= monthBeginDay.getTime() &&
			period.dateTo.toDate().getTime() <= monthEndDay.getTime();
	}

	private isFromOneYear(period: DatePeriod): boolean {
		let d: Date = period.dateFrom.toDate();
		let yearBeginDay: Date = new Date(d.getFullYear(), 0, 1);
		let yearEndDay: Date = new Date(d.getFullYear() + 1, 1, 0, 0, 0, -1);

		return period.dateFrom.toDate().getTime() >= yearBeginDay.getTime() &&
			period.dateTo.toDate().getTime() <= yearEndDay.getTime();
	}

	private isSingleDay(period: DatePeriod): boolean {
		return period.dateFrom.toDate().toDateString() === period.dateTo.toDate().toDateString();
	}

	private uniteDays(period: DatePeriod): string {
		let showDays = !this.isIntegerNumberOfMonths(period);
		let monthDayFrom = period.dateFrom.toDate().getDate();
		let monthDayTo = !this.isSingleDay(period) ? period.dateTo.toDate().getDate() : 0;

		return showDays ? (monthDayTo > 0 ? monthDayFrom + ' - ' + monthDayTo : monthDayFrom + '') : '';
	}
}
