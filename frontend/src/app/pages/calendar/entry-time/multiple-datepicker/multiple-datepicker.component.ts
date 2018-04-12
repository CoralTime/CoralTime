import { Component, EventEmitter, Output, Input } from '@angular/core';
import { DateUtils, Time, TimeEntry } from '../../../../models/calendar';
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-multiple-datepicker',
	templateUrl: 'multiple-datepicker.component.html'
})

export class MultipleDatepickerComponent {
	@Input() firstDayOfWeek: number;
	@Input() timeActual: Time;
	@Input() timeEstimated: Time;
	@Input() timeEntry: TimeEntry;

	@Output() onSubmit: EventEmitter<string[]> = new EventEmitter();

	dateList: string[];
	isCalendarShown: boolean = true;

	dateOnChange(date: Moment[]): void {
		this.dateList = [];
		date.forEach((m: Moment) => {
			this.dateList.push(DateUtils.formatDateToString(m));
		});
	}

	submit(): void {
		this.isCalendarShown = false;
		this.onSubmit.emit(this.dateList);
	}
}
