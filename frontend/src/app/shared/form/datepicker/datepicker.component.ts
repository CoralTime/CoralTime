import {
	Component, Input, Output, EventEmitter, forwardRef, HostBinding, ViewChild, AfterContentInit
} from '@angular/core';
import { IDatePickerDirectiveConfig, DatePickerComponent } from 'ng2-date-picker';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { IDay } from 'ng2-date-picker/day-calendar/day.model';
import { WeekDays } from 'ng2-date-picker/common/types/week-days.type';
import * as moment from 'moment';
import Moment = moment.Moment;

const WEEK_DAYS: WeekDays[] = ['su', 'mo', 'tu', 'we', 'th', 'fr', 'st'];

export const INPUT_CONTROL_VALUE_ACCESSOR: any = {
	provide: NG_VALUE_ACCESSOR,
	useExisting: forwardRef(() => DatepickerComponent),
	multi: true
};

@Component({
	selector: 'ct-datepicker',
	templateUrl: 'datepicker.component.html',
	providers: [INPUT_CONTROL_VALUE_ACCESSOR],
	host: {
		'(document:keydown)': 'onKeyDown($event)'
	}
})

export class DatepickerComponent implements ControlValueAccessor, AfterContentInit {
	@HostBinding('class.ct-datepicker') addClass: boolean = true;
	@Input() config: IDatePickerDirectiveConfig;
	@Input() date: any;
	@Input() disabledDays: string[] = [];
	@Input() displayDate: any;
	@Input() firstDayOfWeek: number = 1;
	@Input() multiselect: boolean = false;
	@Input() required: boolean = false;

	@ViewChild('dayPicker') datePicker: DatePickerComponent;
	@Output() closed: EventEmitter<void> = new EventEmitter<void>();
	@Output() dateAPI: EventEmitter<DatePickerComponent> = new EventEmitter();
	@Output() dateChanged: EventEmitter<string[] | Moment[]> = new EventEmitter();
	@Output() dateClicked: EventEmitter<Moment> = new EventEmitter();

	private _disabled: boolean = false;
	@Input()
	get disabled(): boolean {
		return this._disabled;
	}

	set disabled(value) {
		this._disabled = coerceBooleanProperty(value);
	}

	private _controlValueAccessorChangeFn: (value: any) => void = () => {};
	private onTouched: () => any = () => {};

	constructor() {
	}

	ngAfterContentInit() {
		this.date = this.convertValueToArrayOfString(this.date);
		this.config = {
			allowMultiSelect: this.multiselect,
			firstDayOfWeek: WEEK_DAYS[this.firstDayOfWeek],
			format: 'YYYY-MM-DD',
			weekdayNames: {mo: 'Mo', tu: 'Tu', we: 'We', th: 'Th', fr: 'Fr', sa: 'Sa', su: 'Su'}
		};
		this.dateAPI.emit(this.datePicker);
		setTimeout(() => {
			this.datePicker.api.open();
			this.datePicker.onBodyClick = this.onBodyClick.bind(this.datePicker);
			this.datePicker.dayCalendarRef.isDisabledDay = this.isDateDisabled.bind(this);
			this.datePicker.dayCalendarRef.dayClicked = this.dayClicked.bind(this);
		}, 0);
	}

	/**
	 * Rewrite default methods
	 */

	onBodyClick(this): void {
		this.hideStateHelper = false;
	}

	isDateDisabled(day: IDay): boolean {
		if (this.config.isDayDisabledCallback) {
			return this.config.isDayDisabledCallback(day.date);
		}

		if (this.isDateInDisabledList(day, this.disabledDays)) {
			return true;
		}

		if (this.config.min && day.date.isBefore(this.config.min, 'day')) {
			return true;
		}

		return !!(this.config.max && day.date.isAfter(this.config.max, 'day'));
	}

	isDateInDisabledList(day: IDay, disabledList: string[]): boolean {
		if (!disabledList) {
			return false;
		}

		let result: boolean = false;
		disabledList.forEach((date: string) => {
			if (moment(day.date).format('YYYY-MM-DD') === moment(date).format('YYYY-MM-DD')) {
				result = true;
			}
		});

		return result;
	}

	dayClicked(day: IDay): void {
		let ref = this.datePicker.dayCalendarRef;
		this.dateClicked.emit(day.date);
		ref.selected = ref.utilsService.updateSelected(ref.componentConfig.allowMultiSelect, ref.selected, day);
		ref.weeks = ref.dayCalendarService.generateMonthArray(ref.componentConfig, ref.currentDateView, ref.selected);
		ref.onSelect.emit(day);
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	writeValue(date: Date) {
		if (this.date) {
			this._controlValueAccessorChangeFn(this.date);
		}
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnChange(fn: (value: any) => void) {
		this._controlValueAccessorChangeFn = fn;
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	registerOnTouched(fn: any) {
		this.onTouched = fn;
	}

	/**
	 * Implemented as part of ControlValueAccessor.
	 */
	setDisabledState(isDisabled: boolean) {
		this.disabled = isDisabled;
	}

	_onInputBlur() {
		this.onTouched();
	}

	onKeyDown(event: KeyboardEvent) {
		if (event.key === 'Escape') {
			this.closed.emit();
		}
	}

	changeSelectedDate(date: string[]): void {
		this._controlValueAccessorChangeFn(this.date);
		this.dateChanged.emit(date);
	}

	dateToString(date: Date | Moment | string): string {
		if (date instanceof Date) {
			return date.toISOString();
		}
		return moment(date).toISOString();
	}

	private convertValueToArrayOfString(date: any): string[] {
		if (!this.multiselect) {
			date = date ? [date] : [];
		} else {
			date = date ? [...date] : [];
		}
		return date.map(date => this.dateToString(date));
	}
}
