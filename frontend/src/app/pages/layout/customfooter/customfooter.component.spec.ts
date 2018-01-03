/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomfooterComponent } from './customfooter.component';

describe('CustomfooterComponent', () => {
	let component: CustomfooterComponent;
	let fixture: ComponentFixture<CustomfooterComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [CustomfooterComponent]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(CustomfooterComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});