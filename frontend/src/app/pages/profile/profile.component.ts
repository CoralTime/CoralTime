import { Component, OnInit } from '@angular/core';
import { AuthUser } from '../../core/auth/auth-user';
import { AuthService } from '../../core/auth/auth.service';
import { Subscription } from 'rxjs/Subscription';
import { ImpersonationService } from '../../services/impersonation.service';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';
import { ProfileProjectMember, ProfileProjects, ProfileService } from '../../services/profile.service';

@Component({
	selector: 'ct-profile',
	templateUrl: 'profile.component.html'
})

export class ProfileComponent implements OnInit {
	authUser: AuthUser;
	impersonationName: string = null;
	impersonationUser: User;
	impersonationId: number = null;
	projects: ProfileProjects[];
	userId: number;
	userModel: User = new User();

	private subscriptionImpersonation: Subscription;

	constructor(private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private profileService: ProfileService,
	            private usersService: UsersService) {
	}

	ngOnInit() {
		this.authUser = this.authService.getAuthUser();
		this.getUserPicture();
		this.userId = this.impersonationService.impersonationId || this.authUser.id;

		this.usersService.getUserById(this.userId).subscribe((user: User) => {
			this.userModel = user;
		});
		this.getProjects();
	}

	private getUserPicture(): void {
		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			if (this.impersonationService.impersonationMember) {
				this.impersonationUser = this.impersonationService.impersonationUser;
				this.impersonationName = this.impersonationService.impersonationUser.fullName;
				this.impersonationId = this.impersonationService.impersonationId;
			} else {
				this.impersonationUser = null;
				this.impersonationName = null;
				this.impersonationId = null;
			}
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
			this.projects[index].memberList = this.sortList(members, 'fullName');
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
