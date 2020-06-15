import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { PagedResult } from '../../../services/odata';
import { MemberAction } from '../../../models/member-action';
import { AclService } from '../../../core/auth/acl.service';
import { ROWS_ON_PAGE } from '../../../core/constant.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { MemberActionsService } from '../../../services/member-action.service';
import { NotificationService } from '../../../core/notification.service';

@Component({
	selector: 'ct-member-actions',
	templateUrl: 'member-actions-grid.component.html'
})

export class MemberActionsComponent implements OnInit {
	isAllActions: boolean = false;
	filterStr: string = '';
	pagedResult: PagedResult<MemberAction>;
	resizeObservable: Subject<any> = new Subject();
	updatingGrid: boolean = false;

	@ViewChild('pageContainer') pageContainer: ElementRef;

	private lastEvent: any;
	private subject = new Subject<any>();

	constructor(private aclService: AclService,
	            private impersonationService: ImpersonationService,
	            private notificationService: NotificationService,
	            private memberActionsService: MemberActionsService) {
	}

	ngOnInit() {
		this.getMemberActions();
	}

	onEndScroll(): void {
		if (!this.isAllActions) {
			this.loadLazy();
		}
	}

	getMemberActions(): void {
		this.subject.debounceTime(500).switchMap(() => {
			return this.memberActionsService.getMemberActions(this.lastEvent, this.filterStr);
		})
			.subscribe((res: PagedResult<MemberAction>) => {
					if (!this.pagedResult || !this.lastEvent.first || this.updatingGrid) {
						this.pagedResult = res;
					} else {
						this.pagedResult.data = this.pagedResult.data.concat(res.data);
					}

					this.lastEvent.first = this.pagedResult.data.length;
					this.updatingGrid = false;
					this.checkIsAllActions();
				},
				() => this.notificationService.danger('Error loading member actions.')
			);
	}

	loadLazy(event = null, updatePage?: boolean): void {
		if (event) {
			this.lastEvent = event;
		}
		if (updatePage) {
			this.updatingGrid = true;
			this.lastEvent.first = 0;
		}
		if (event || updatePage) {
			this.isAllActions = false;
			this.pagedResult = null;
			this.resizeObservable.next(true);
		}
		this.lastEvent.rows = ROWS_ON_PAGE;
		if (!updatePage && this.isAllActions) {
			return;
		}

		this.subject.next({
			event,
			filterStr: this.filterStr
		});
	}

	private checkIsAllActions(): void {
		if (this.pagedResult && this.pagedResult.data.length >= this.pagedResult.count) {
			this.isAllActions = true;
		}
	}

	onResize(): void {
		this.resizeObservable.next();
	}
}
