import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/debounceTime';
import { ProjectsService } from '../../services/projects.service';
import { Project } from '../../models/project';

@Directive({
	selector: '[ctProjectNameValidator][formControlName],[ctProjectNameValidator][formControl],[ctProjectNameValidator][ngModel]',
	providers: [
		{
			provide: NG_ASYNC_VALIDATORS,
			useExisting: forwardRef(() => ProjectNameValidator),
			multi: true
		}
	]
})

export class ProjectNameValidator implements Validator {
	observable: Observable<any>;
	resolve: any = null;
	subject: Subject<string>;

	@Input('ctProjectNameValidator') private project: Project;

	constructor(private projectsService: ProjectsService) {
		this.subject = new Subject();
		this.observable = this.subject
			.debounceTime(300)
			.switchMap((projectName: string) => {
				return this.projectsService.getProjectByName(projectName);
			}).flatMap(project => {
				if (project && (!this.project || project.id !== this.project.id)) {
					return Observable.of({ctProjectNameInvalid: true});
				} else {
					return Observable.of(null);
				}
			});

		this.observable.subscribe((res) => {
			this.resolvePromise(res);
		});
	}

	resolvePromise(result): void {
		if (this.resolve) {
			this.resolve(result);
			this.resolve = null;
		}
	}

	validate(c: AbstractControl): Promise<{[key: string]: any}> {
		this.resolvePromise(null);

		return new Promise(resolve => {
			this.subject.next(c.value);
			this.resolve = resolve;
		});
	}
}
