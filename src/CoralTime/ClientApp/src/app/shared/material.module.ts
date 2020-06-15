import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';

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
