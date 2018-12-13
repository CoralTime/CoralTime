import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { User } from '../../models/user';
import { ArrayUtils } from '../../core/object-utils';
import { ImpersonationService } from '../../services/impersonation.service';
import { ProfileProjectMember, ProfileProjects, ProfileService } from '../../services/profile.service';
import { UserPicService } from '../../services/user-pic.service';
import { numberToHex } from '../../shared/form/color-picker/color-picker.component';
import { SelectComponent } from '../../shared/form/select/select.component';

@Component({
	selector: 'ct-profile',
	templateUrl: 'profile.component.html'
})

export class ProfileComponent implements OnInit {
	avatarUrl: string;
	impersonationUser: User;
	projects: ProfileProjects[];
	resizeObservable: Subject<any> = new Subject();
	userInfo: User;

	constructor(private impersonationService: ImpersonationService,
	            private profileService: ProfileService,
	            private route: ActivatedRoute,
	            private userPicService: UserPicService) {
	}

	ngOnInit() {
		this.route.data.forEach((data: { user: User }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
		});
		this.getAvatar();
	}

	getAvatar(): void {
		this.userPicService.loadUserPicture(this.userInfo.id).subscribe((avatarUrl: string) => {
			this.avatarUrl = avatarUrl;
		});
	}

	loadLazy(event = null): void {
		this.profileService.getProjects().subscribe((projects: ProfileProjects[]) => {
			this.projects = [...ArrayUtils.sortByField(projects, event.sortField, event.sortOrder)];
		});
	}

	setManagersString(managersList: string[]): string {
		if (managersList.length) {
			return managersList.join(', ');
		}

		return '';
	}

	toggleMembersShown(select: SelectComponent, project: ProfileProjects, index: number): void {
		if (!this.projects[index].memberList.length) {
			project.isMemberLoading = true;
			this.getProjectMembers(this.projects[index].id, index).then(() => {
				project.isMemberLoading = false;
				select.toggleSelect();
			});
		} else {
			select.toggleSelect();
		}
	}

	private getProjectMembers(projectId: number, index: number): Promise<void> {
		return this.profileService.getProjectMembers(projectId)
			.toPromise()
			.then((members: ProfileProjectMember[]) => {
				this.projects[index].memberList = this.sortList(members, 'memberName');
			});
	}

	private sortList(list: any[], sortingField: string): any[] {
		return list.sort((a, b) => a[sortingField].toLowerCase() < b[sortingField].toLowerCase() ? -1 : 1);
	}

	// GENERAL

	numberToHex(value: number): string {
		return numberToHex(value);
	}
}
