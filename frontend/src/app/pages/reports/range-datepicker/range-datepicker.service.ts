import { Injectable } from '@angular/core';
import { CalendarService } from '../../../services/calendar.service';
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

@Injectable()
export class RangeDatepickerService {
	private DATE_PERIOD: any;
	private currentDay: Date = moment().startOf('day').toDate();

	constructor(private service: CalendarService) {
	}

	getDatePeriodList(): any {
		return this.DATE_PERIOD;
	}

	setDatePeriodList(firstDayOfWeek: number): void {
		this.DATE_PERIOD = {
			'Today': this.getToday(),
			'This Week': this.getCurrentWeek(firstDayOfWeek),
			'This Month': this.getCurrentMonth(),
			'This Year': this.getCurrentYear(),
			'Yesterday': this.getYesterday(),
			'Last Week': this.getLastWeek(firstDayOfWeek),
			'Last Month': this.getLastMonth(),
			'Last Year': this.getLastYear()
		};
	}

	setDateStringPeriod(period: DatePeriod): string {
		for (let prop in this.DATE_PERIOD) {
			if (this.isDatePeriodEqual(period, this.DATE_PERIOD[prop])) {
				return prop;
			}
		}

		if (this.isFromPeriod(period, 'This Week')) {
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
			return this.isFromPeriod(period, 'This Year') ? dateString : dateString + ' ' + yearFrom;
		}

		monthFormat = this.isIntegerNumberOfMonths(period) ? 'long' : 'short';
		let monthDayFrom = period.dateFrom.toDate().getDate();
		let monthDayTo = period.dateTo.toDate().getDate();
		monthNameFrom = period.dateFrom.toDate().toLocaleString('en-us', {month: monthFormat});
		let monthNameTo = period.dateTo.toDate().toLocaleString('en-us', {month: monthFormat});

		if (this.isFromOneYear(period)) {
			dateString = monthNameFrom + ' ' + monthDayFrom + ' - ' + monthNameTo + ' ' + monthDayTo;
			return this.isFromPeriod(period, 'This Year') ? dateString : dateString + ', ' + period.dateFrom.year();
		}

		let yearTo = period.dateTo.year();

		return monthNameFrom + ' ' + monthDayFrom + ', ' + yearFrom + ' - ' + monthNameTo + ' ' + monthDayTo + ', ' + yearTo;
	}

	isDatePeriodEqual(a: DatePeriod, b: DatePeriod): boolean {
		if (
			a.dateFrom.toDate().toDateString() === b.dateFrom.toDate().toDateString()
			&& a.dateTo.toDate().toDateString() === b.dateTo.toDate().toDateString()
		) {
			return true;
		}

		return false;
	}

	isIntegerNumberOfMonths(period: DatePeriod): boolean {
		let nextDay = (d => new Date(d.setDate(d.getDate() + 1)))(period.dateTo.toDate());
		return period.dateFrom.date() === 1 && nextDay.getDate() === 1;
	}

	private getToday(): DatePeriod {
		return new DatePeriod(moment().startOf('day'));
	}

	private getCurrentWeek(firstDayOfWeek: number): DatePeriod {
		let weekBeginDay: Date = this.service.getWeekBeginning(this.getToday().dateFrom.toDate(), firstDayOfWeek);
		let weekEndDay: Date = new Date(weekBeginDay.getTime() + 86400 * 6 * 1000);

		return new DatePeriod(moment(weekBeginDay), moment(weekEndDay));
	}

	private getCurrentMonth(): DatePeriod {
		let monthBeginDay: Date = new Date(this.currentDay.getFullYear(), this.currentDay.getMonth(), 1);
		let monthEndDay: Date = new Date(this.currentDay.getFullYear(), this.currentDay.getMonth() + 1, 0);

		return new DatePeriod(moment(monthBeginDay), moment(monthEndDay));
	}

	private getCurrentYear(): DatePeriod {
		let yearBeginDay: Date = new Date(this.currentDay.getFullYear(), 0, 1);
		let yearEndDay: Date = new Date(this.currentDay.getFullYear() + 1, 0, 0);

		return new DatePeriod(moment(yearBeginDay), moment(yearEndDay));
	}

	private getYesterday(): DatePeriod {
		let yesterday = (d => new Date(d.setDate(d.getDate() - 1)))(new Date);
		return new DatePeriod(moment(yesterday).startOf('day'));
	}

	private getLastWeek(firstDayOfWeek: number): DatePeriod {
		let currentWeekBeginDay: Date = this.service.getWeekBeginning(this.getToday().dateFrom.toDate(), firstDayOfWeek);
		let weekBeginDay: Date = new Date(currentWeekBeginDay.getTime() - 86400 * 7 * 1000);
		let weekEndDay: Date = new Date(weekBeginDay.getTime() + 86400 * 6 * 1000);

		return new DatePeriod(moment(weekBeginDay), moment(weekEndDay));
	}

	private getLastMonth(): DatePeriod {
		let monthBeginDay: Date = new Date(this.currentDay.getFullYear(), this.currentDay.getMonth() - 1, 1);
		let monthEndDay: Date = new Date(this.currentDay.getFullYear(), this.currentDay.getMonth(), 0);

		return new DatePeriod(moment(monthBeginDay), moment(monthEndDay));
	}

	private getLastYear(): DatePeriod {
		let yearBeginDay: Date = new Date(this.currentDay.getFullYear() - 1, 0, 1);
		let yearEndDay: Date = new Date(this.currentDay.getFullYear(), 0, 0);

		return new DatePeriod(moment(yearBeginDay), moment(yearEndDay));
	}

	private isSingleDay(period: DatePeriod): boolean {
		return period.dateFrom.toDate().toDateString() === period.dateTo.toDate().toDateString();
	}

	private isFromPeriod(period: DatePeriod, periodName: string): boolean {
		return period.dateFrom.toDate().getTime() >= this.DATE_PERIOD[periodName].dateFrom.toDate().getTime() &&
			period.dateTo.toDate().getTime() <= this.DATE_PERIOD[periodName].dateTo.toDate().getTime();
	}

	private isFromOneMonth(period: DatePeriod): boolean {
		let d: Date = period.dateFrom.toDate();
		let monthBeginDay: Date = new Date(d.getFullYear(), d.getMonth(), 1);
		let monthEndDay: Date = new Date(d.getFullYear(), d.getMonth() + 1, 0);

		return period.dateFrom.toDate().getTime() >= monthBeginDay.getTime() &&
			period.dateTo.toDate().getTime() <= monthEndDay.getTime();
	}

	private isFromOneYear(period: DatePeriod): boolean {
		let d: Date = period.dateFrom.toDate();
		let yearBeginDay: Date = new Date(d.getFullYear(), 0, 1);
		let yearEndDay: Date = new Date(d.getFullYear() + 1, 0, 0);

		return period.dateFrom.toDate().getTime() >= yearBeginDay.getTime() &&
			period.dateTo.toDate().getTime() <= yearEndDay.getTime();
	}

	private uniteDays(period: DatePeriod): string {
		let showDays = !this.isIntegerNumberOfMonths(period);
		let monthDayFrom = period.dateFrom.toDate().getDate();
		let monthDayTo = !this.isSingleDay(period) ? period.dateTo.toDate().getDate() : 0;

		return showDays ? (monthDayTo > 0 ? monthDayFrom + ' - ' + monthDayTo : monthDayFrom + '') : '';
	}
}
