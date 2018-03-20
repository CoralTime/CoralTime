import { Injectable } from '@angular/core';
import { Subject, Observable, Subscription } from 'rxjs';
import * as Rx from 'rxjs'

@Injectable()
export class LoadingIndicatorService {
	private started = false;
	private status = 0;
	private subject: Subject<number> = new Subject<number>();
	private readonly startSize: number = 0.02;
	private completeSubscription: Subscription;
	private incrimentSubscription: Subscription;

	constructor() {
	}

	complete(): void {
		this.set(1);
		if (this.incrimentSubscription) {
			this.incrimentSubscription.unsubscribe();
		}

		this.completeSubscription = Observable.timer(100).subscribe(() => {
			this.set(0);
			this.started = false;
		});
	}

	getStatus(): Observable<number | {}> {
		let source = Rx.Observable.empty();

		return source
			.startWith(this.status)
			.concat(this.subject)
			.distinctUntilChanged();
	}

	start(): void {
		if (this.completeSubscription) {
			this.completeSubscription.unsubscribe();
		}

		if (this.started) {
			return;
		}

		this.started = true;

		this.set(this.startSize);

		if (this.incrimentSubscription) {
			this.incrimentSubscription.unsubscribe();
		}

		this.incrimentSubscription = Rx.Observable
			.interval(50)
			.timeInterval().subscribe(() => {
				this.inc();
			});
	}

	set(status: number): void {
		if (!this.started) {
			return;
		}

		this.status = status;

		this.subject.next(this.status);
	}

	inc(): void {
		if (this.status >= 1) {
			return;
		}

		let rnd = 0;

		// TODO: do this mathmatically instead of through conditions

		if (this.status >= 0 && this.status < 0.25) {
			// Start out between 3 - 6% increments
			rnd = (Math.random() * (5 - 3 + 1) + 3) / 100;
		} else if (this.status >= 0.25 && this.status < 0.65) {
			// increment between 0 - 3%
			rnd = (Math.random() * 3) / 100;
		} else if (this.status >= 0.65 && this.status < 0.9) {
			// increment between 0 - 2%
			rnd = (Math.random() * 2) / 100;
		} else if (this.status >= 0.9 && this.status < 0.99) {
			// finally, increment it .5 %
			rnd = 0.005;
		} else {
			// after 99%, don't increment:
			rnd = 0;
		}

		let status = this.status + rnd;
		this.set(status);
	}
}
