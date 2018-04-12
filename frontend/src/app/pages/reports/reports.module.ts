import { SharedModule } from '../../shared/shared.module';
import { NgModule } from '@angular/core';
import { ReportsComponent } from './reports.component';
import { ReportsRoutingModule } from './reports-routing.module';
import { ReportsService } from '../../services/reposts.service';
import { ReportsGridComponent } from './reports-data/reports-grid.component';
import { RangeDatepickerComponent } from './range-datepicker/range-datepicker.component';
import { CalendarService } from '../../services/calendar.service';
import { RangeDatepickerService } from './range-datepicker/range-datepicker.service';
import { ReportsSendComponent } from './reports-send/reports-send.component';
import { ReportsSendFormComponent } from './reports-send/form/reports-send-form.component';
import { EmailsEqualValidatorDirective } from './reports-send/form/emails-equal-validator.directive';
import { EmailInvalidValidatorDirective } from './reports-send/form/email-invalid-validator.directive';
import { ReportsQueryFormComponent } from './reports-query-form/reports-query-form.component';
import { ConfirmationComponent } from '../../shared/confirmation/confirmation.component';

@NgModule({
	imports: [
		SharedModule,
		ReportsRoutingModule,
	],
	declarations: [
		ReportsComponent,
		ReportsGridComponent,
		RangeDatepickerComponent,
		ReportsSendComponent,
		ReportsSendFormComponent,
		EmailsEqualValidatorDirective,
		EmailInvalidValidatorDirective,
		ReportsQueryFormComponent
	],
	entryComponents: [
		ReportsSendComponent,
		ReportsQueryFormComponent,
		ConfirmationComponent
	],
	providers: [
		CalendarService,
		RangeDatepickerService,
		ReportsService,
	],
	exports: [
		ReportsComponent
	]
})

export class ReportsModule {
}
