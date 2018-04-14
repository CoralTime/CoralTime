import { NgModule } from '@angular/core';
import {
	MatButtonModule, MatCheckboxModule, MatDialogModule, MatIconModule, MatInputModule, MatListModule,
	MatProgressSpinnerModule, MatRadioModule, MatSelectModule, MatSnackBarModule
} from '@angular/material';

@NgModule({
	imports: [
		MatButtonModule,
		MatCheckboxModule,
		MatDialogModule,
		MatIconModule,
		MatInputModule,
		MatListModule,
		MatProgressSpinnerModule,
		MatRadioModule,
		MatSelectModule,
		MatSnackBarModule
	],
	exports: [
		MatButtonModule,
		MatCheckboxModule,
		MatDialogModule,
		MatIconModule,
		MatInputModule,
		MatListModule,
		MatProgressSpinnerModule,
		MatRadioModule,
		MatSelectModule,
		MatSnackBarModule
	]
})

export class MaterialModule {
}
