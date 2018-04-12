import { Component, OnInit } from '@angular/core';
import { ImpersonationService } from '../../services/impersonation.service';
import { User } from '../../models/user';
import { ProfileProjectMember, ProfileProjects, ProfileService } from '../../services/profile.service';
import { ActivatedRoute } from '@angular/router';
import { UserPicService } from '../../services/user-pic.service';

@Component({
	selector: 'ct-profile',
	templateUrl: 'profile.component.html'
})

export class ProfileComponent implements OnInit {
	avatarUrl: string;
	impersonationUser: User;
	projects: ProfileProjects[];
	userInfo: User = new User();

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
		this.getProjects();
	}

	getAvatar(): void {
		this.userPicService.loadUserPicture(this.userInfo.id).subscribe((avatarUrl: string) => {
			this.avatarUrl = avatarUrl;
		});
	}

	getProjects(): void {
		this.profileService.getProjects().subscribe((projects: ProfileProjects[]) => {
			this.projects = this.sortList(projects, 'name');
		});
	}

	sortList(list: any[], sortingField: string): any[] {
		return list.sort((a, b) => a[sortingField].toLowerCase() < b[sortingField].toLowerCase() ? -1 : 1);
	}

	getProjectMembers(projectId: number, index: number): void {
		this.profileService.getProjectMembers(projectId).subscribe((members: ProfileProjectMember[]) => {
			this.projects[index].memberList = this.sortList(members, 'memberName');
		});
	}

	setManagersString(managersList: string[]): string {
		if (managersList.length) {
			let plural = managersList.length - 1 ? 's are ' : ' is ';
			return 'Your manager' + plural + managersList.join(', ');
		} else {
			return '';
		}
	}

	setMemberCountString(memberCount: number): string {
		if (memberCount === 1) {
			return 'You are the only one member on the project';
		} else {
			return memberCount + ' members on the project';
		}
	}

	toggleMembersShown(index: number): void {
		if (!this.projects[index].memberList.length) {
			this.getProjectMembers(this.projects[index].id, index);
		}
		this.projects[index].isMemberListShown = !this.projects[index].isMemberListShown;
	}
}
