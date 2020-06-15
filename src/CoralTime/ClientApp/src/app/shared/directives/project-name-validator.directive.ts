import { Directive, forwardRef, Input } from '@angular/core';
import { Validator, AbstractControl, NG_ASYNC_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
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
	@Input('ctProjectNameValidator') project: Project;

	constructor(private projectsService: ProjectsService) {
	}

	validate(control: AbstractControl): Observable<{ [key: string]: any }> {
		return control.valueChanges
			.debounceTime(500)
			.take(1)
			.switchMap(() => {
				return this.projectsService.getProjectByName(control.value);
			})
			.map(project => {
				if (project && (!this.project || project.id !== this.project.id)) {
					return {ctProjectNameInvalid: true};
				}

				return null;
			})
			.first()
	}
}
