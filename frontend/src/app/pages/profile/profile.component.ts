import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthUser } from '../../core/auth/auth-user';
import { AuthService } from '../../core/auth/auth.service';
import { Subscription } from 'rxjs/Subscription';
import { ImpersonationService } from '../../services/impersonation.service';
import { User } from '../../models/user';
import { ProfileProjectMember, ProfileProjects, ProfileService } from '../../services/profile.service';
import { Avatar, UserPicService } from '../../services/user-pic.service';
import { ActivatedRoute } from '@angular/router';

@Component({
	selector: 'ct-profile',
	templateUrl: 'profile.component.html'
})

export class ProfileComponent implements OnInit, OnDestroy {
	authUser: AuthUser;
	impersonationUser: User;
	projects: ProfileProjects[];
	userInfo: User = new User();
	avatarUrl: string;

	private subscriptionImpersonation: Subscription;

	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private profileService: ProfileService,
	            private route: ActivatedRoute,
	            private userPicService: UserPicService) {
	}

	ngOnInit() {
		this.authUser = this.authService.getAuthUser();

		this.route.data.forEach((data: { user: User }) => {
			this.userInfo = this.impersonationService.impersonationUser || data.user;
		});

		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			this.impersonationUser = this.impersonationService.impersonationUser;
		});

		this.getUserPicture();
		this.getProjects();
	}

	private getUserPicture(): void {
		this.userPicService.getUserPicture(this.impersonationUser ? this.impersonationUser.id : this.userInfo.id, true)
			.subscribe((avatar: Avatar) => {
				this.avatarUrl = avatar.avatarUrl;
			});
	}

	private getProjects(): void {
		this.profileService.getProjects().subscribe((projects: ProfileProjects[]) => {
			this.projects = this.sortList(projects, 'name');
		});
	}

	private sortList(list: any[], sortingField: string): any[] {
		return list.sort((a, b) => a[sortingField].toLowerCase() < b[sortingField].toLowerCase() ? -1 : 1);
	}

	private getProjectMembers(projectId: number, index: number): void {
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

	ngOnDestroy() {
		this.subscriptionImpersonation.unsubscribe();
	}
}
