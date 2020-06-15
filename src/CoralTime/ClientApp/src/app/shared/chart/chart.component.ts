import { Component, HostBinding, Input } from '@angular/core';

@Component({
	selector: 'ct-chart',
	template: `
        <div class="ct-chart-block">
            <div class="ct-chart-bar"
                 [class.ct-chart-over]="isWorkingHoursOver()"
                 [style.width]="calcWorkingHours() + '%'"></div>
        </div>
	`
})

export class ChartComponent {
	@Input() value: number = 0;
	@Input() totalValue: number;
	@HostBinding('class.ct-chart') addClass: boolean = true;

	calcWorkingHours(): number {
		this.totalValue = this.totalValue || this.value || 1;
		return Math.min(this.value, this.totalValue) / this.totalValue * 100;
	}

	isWorkingHoursOver(): boolean {
		return this.value > this.totalValue;
	}
}
