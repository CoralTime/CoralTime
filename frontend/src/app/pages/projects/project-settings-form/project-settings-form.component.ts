import { Component, OnInit, Output, EventEmitter, ChangeDetectionStrategy, Input } from '@angular/core';

import { ProjectsService } from '../../../services/projects.service';
import { Project } from '../../../models/project';

class ProjectSettingsModel {
	color: number;
	daysBeforeStopEditTimeEntries: number;
	id: number;
	isNotificationEnabled: boolean;
	isPrivate: boolean;
	isTimeLockEnabled: boolean;
	lockPeriod: number;
	notificationDay: number;

	static fromProject(project: Project) {
		let instance = new this;
		instance.color = project.color;
		instance.daysBeforeStopEditTimeEntries = project.daysBeforeStopEditTimeEntries;
		instance.id = project.id;
		instance.isNotificationEnabled = project.isNotificationEnabled;
		instance.isPrivate = project.isPrivate;
		instance.isTimeLockEnabled = project.isTimeLockEnabled;
		instance.lockPeriod = project.lockPeriod ? project.lockPeriod : 1;
		instance.notificationDay = project.notificationDay;

		return instance;
	}

	toProject(): Project {
		return new Project({
			color: this.color,
			id: this.id,
			daysBeforeStopEditTimeEntries: this.daysBeforeStopEditTimeEntries,
			isNotificationEnabled: this.isNotificationEnabled,
			isPrivate: this.isPrivate,
			isTimeLockEnabled: this.isTimeLockEnabled,
			lockPeriod: this.lockPeriod,
			notificationDay: this.notificationDay
		});
	}
}

@Component({
	selector: 'ct-project-settings-form',
	templateUrl: 'project-settings-form.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})

export class ProjectSettingsFormComponent implements OnInit {
	@Input() project: Project;
	@Output() onSaved = new EventEmitter();

	lockPeriodModel: any;
	model: ProjectSettingsModel;
	numberMask = [/[1-9]/, /\d/];

	lockPediods = [
		{value: 2, viewValue: 'Month'},
		{value: 1, viewValue: 'Week'}
	];

	constructor(private projectsService: ProjectsService) {
	}

	ngOnInit() {
		this.model = ProjectSettingsModel.fromProject(this.project);
		this.lockPeriodModel = this.lockPediods.filter((period) => period.value === this.model.lockPeriod)[0];
	}

	lockPeriodOnChange(): void {
		this.model.lockPeriod = this.lockPeriodModel.value;
	}

	save(): void {
		let updatedProject = this.model.toProject();
		let observable = this.projectsService.odata.Put(updatedProject, updatedProject.id.toString());

		observable.subscribe(res => this.onSaved.emit());
	}
}
