import { NgModule } from '@angular/core';
import {
	MatButtonModule, MatCheckboxModule, MatDialogModule, MatIconModule, MatInputModule, MatListModule,
	MatProgressSpinnerModule, MatRadioModule, MatSelectModule, MatSnackBarModule, MatExpansionModule
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
		MatSnackBarModule,
		MatExpansionModule
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
		MatSnackBarModule,
		MatExpansionModule
	]
})

export class MaterialModule {
}
