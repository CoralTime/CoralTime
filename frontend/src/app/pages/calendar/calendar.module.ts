import { NgModule } from '@angular/core';
import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarComponent } from './calendar.component';
import { CalendarService } from '../../services/calendar.service';
import { CalendarTaskComponent } from './calendar-views/calendar-task/calendar-task.component';
import { CalendarDailyViewComponent } from './calendar-views/daily-view/daily-view.component';
import { CalendarWeeklyViewComponent } from './calendar-views/weekly-view/weekly-view.component';
import { CalendarDayComponent } from './calendar-views/calendar-day/calendar-day.component';
import { EntryTimeModule } from './entry-time/entry-time.module';
import { SharedModule } from '../../shared/shared.module';
import { CalendarProjectsService } from './calendar-projects.service';
import { ConfirmationComponent } from '../../shared/confirmation/confirmation.component';
import { DragDropModule } from 'primeng/primeng';

@NgModule({
	imports: [
		CalendarRoutingModule,
		SharedModule,
		EntryTimeModule,
		DragDropModule
	],
	declarations: [
		CalendarComponent,
		CalendarDayComponent,
		CalendarTaskComponent,
		CalendarDailyViewComponent,
		CalendarWeeklyViewComponent
	],
	entryComponents: [
		ConfirmationComponent
	],
	providers: [
		CalendarService,
		CalendarProjectsService
	],
	exports: [
		CalendarComponent
	]
})

export class CalendarModule {
}
