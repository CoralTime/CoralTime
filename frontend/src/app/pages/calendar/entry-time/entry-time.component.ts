import { Component, ViewChild, ElementRef, Input, EventEmitter, Output, OnDestroy } from '@angular/core';
import { TimeEntry } from '../../../models/calendar';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmationComponent } from '../../../shared/confirmation/confirmation.component';
import { CalendarService } from '../../../services/calendar.service';
import { ObjectUtils } from '../../../core/object-utils';

@Component({
	selector: 'ct-entry-time',
	templateUrl: 'entry-time.component.html'
})

export class EntryTimeComponent implements OnDestroy {
	isOpen: boolean = false;
	isOpenLeft: boolean = false;
	isOpenRight: boolean = false;
	isOpenMobile: boolean = false;
	isDirectionTop: boolean = false;
	canClose: boolean = true;

	@Input() timeEntry: TimeEntry;
	@Output() deleted: EventEmitter<void> = new EventEmitter<void>();
	@Output() timerUpdated: EventEmitter<void> = new EventEmitter<void>();
	@ViewChild('entryForm') entryForm;

	private dialogRef: MatDialogRef<ConfirmationComponent>;

	constructor(private calendarService: CalendarService,
	            private dialog: MatDialog,
	            private elementRef: ElementRef) {
	}

	toggleEntryTimeForm(): void {
		this.changeCloseParameter();

		if (!this.isOpen && !this.calendarService.isTimeEntryFormOpened) {
			this.openTimeEntryForm();
		} else {
			this.closeTimeEntryForm();
		}
	}

	closeTimeEntryForm(showConfirmationDialog?: boolean): void {
		if (!this.canClose) {
			return;
		}

		if (!this.timeEntry.id) {
			if (showConfirmationDialog && this.isTimeEntryFormChanged(this.timeEntry, this.entryForm.currentTimeEntry)) {
				this.openConfirmationDialog();
			} else {
				this.calendarService.isTimeEntryFormOpened = false;
				this.deleted.emit();
			}
			return;
		}

		if (showConfirmationDialog && this.isTimeEntryFormChanged(this.timeEntry, this.entryForm.currentTimeEntry)) {
			this.openConfirmationDialog();
		} else {
			this.isOpen = false;
			this.calendarService.isTimeEntryFormOpened = false;
		}
	}

	openTimeEntryForm(): void {
		this.isOpen = true;
		this.calendarService.isTimeEntryFormOpened = true;
		this.isOpenRight = this.isRightSideClear(this.elementRef.nativeElement);
		this.isOpenLeft = !this.isOpenRight && this.isLeftSideClear(this.elementRef.nativeElement);
		this.isOpenMobile = !this.isOpenRight && !this.isOpenLeft;
		this.isDirectionTop = !this.isBottomClear(this.elementRef.nativeElement) && this.isTopClear(this.elementRef.nativeElement);

		if (!this.isOpenMobile) {
			this.scrollWindow(this.elementRef.nativeElement);
		}
	}

	openConfirmationDialog(): void {
		this.dialogRef = this.dialog.open(ConfirmationComponent);
		this.dialogRef.componentInstance.message = 'You have modified this work item. Close and lose changes?';

		this.dialogRef.componentInstance.onSubmit.subscribe((confirm: boolean) => {
			if (confirm) {
				this.closeTimeEntryForm();
			}

			this.dialogRef.close();
		});
	}

	deleteTimeEntry(): void {
		this.deleted.emit();
	}

	ngOnDestroy() {
		this.calendarService.isTimeEntryFormOpened = false;
	}

	private isRightSideClear(el: HTMLElement): boolean {
		return window.innerWidth > el.getBoundingClientRect().right + 300;
	}

	private isLeftSideClear(el: HTMLElement): boolean {
		return el.getBoundingClientRect().left > 300;
	}

	private isBottomClear(el: HTMLElement): boolean {
		return window.innerHeight > el.getBoundingClientRect().top + 560;
	}

	private isTopClear(el: HTMLElement): boolean {
		return el.getBoundingClientRect().bottom > 560;
	}

	private changeCloseParameter(): void {
		this.canClose = false;
		setTimeout(() => this.canClose = true, 0);
	}

	private isTimeEntryFormChanged(obj: any, obj2: any): boolean {
		return !ObjectUtils.deepEqualWithEvery(obj, obj2);
	}

	private scrollWindow(el: HTMLElement): void {
		let elTop: number = el.getBoundingClientRect().top;
		if (!this.isDirectionTop && elTop < 195) {
			window.scrollTo({
				left: 0,
				top: elTop + window.scrollY - 195,
				behavior: 'smooth'
			});
		}

		let elBottom: number = el.getBoundingClientRect().bottom;
		if (this.isDirectionTop && elBottom > window.innerHeight) {
			window.scrollTo({
				left: 0,
				top: elBottom - window.innerHeight + window.scrollY + 5,
				behavior: 'smooth'
			});
		}
	}
}
