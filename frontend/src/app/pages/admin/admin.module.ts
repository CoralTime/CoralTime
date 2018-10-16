import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { SharedModule } from '../../shared/shared.module';
import { MemberActionsComponent } from './member-actions-data/member-actions.component';



@NgModule({
	imports: [
		CommonModule,
		AdminRoutingModule,
		SharedModule
	],
	declarations: [
		AdminComponent,
        MemberActionsComponent
	],
    entryComponents: [
        MemberActionsComponent
    ],
	exports: [
		AdminComponent
	]
})

export class AdminModule {
}
