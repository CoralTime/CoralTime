import { Component, EventEmitter, Output, Input } from '@angular/core';
import { DateUtils, TimeEntry } from '../../../../models/calendar';
import { Time } from '../entry-time-form/entry-time-form.component';
import * as moment from 'moment';
import Moment = moment.Moment;

@Component({
	selector: 'ct-multiple-datepicker',
	templateUrl: 'multiple-datepicker.component.html'
})

export class MultipleDatepickerComponent {
	@Input() firstDayOfWeek: number;
	@Input() actualTime: Time;
	@Input() plannedTime: Time;
	@Input() timeEntry: TimeEntry;

	@Output() onSubmit: EventEmitter<Date[]> = new EventEmitter();

	dateList: Date[];
	isCalendarShown: boolean = true;

	dateOnChange(date: Moment[]): void {
		this.dateList = [];
		date.forEach((m: Moment) => {
			this.dateList.push(DateUtils.convertMomentToUTC(m));
		});
	}

	submit(): void {
		this.isCalendarShown = false;
		this.onSubmit.emit(this.dateList);
	}
}
