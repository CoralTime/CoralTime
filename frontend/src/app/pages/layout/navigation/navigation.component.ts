import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { User } from '../../../models/user';
import { AclService } from '../../../core/auth/acl.service';
import { AuthUser } from '../../../core/auth/auth-user';
import { AuthService } from '../../../core/auth/auth.service';
import { AuthGuard } from '../../../core/auth/auth-guard.service';
import { ImpersonationService } from '../../../services/impersonation.service';
import { ProjectsService } from '../../../services/projects.service';
import { UsersService } from '../../../services/users.service';
import { LoadingMaskService } from '../../../shared/loading-indicator/loading-mask.service';

interface MenuItem {
	label?: string;
	icon?: string;
	routerLink?: any;
	permission?: string;
}

const FULL_MANAGE_ITEMS = [
	{
		label: 'Projects',
		icon: 'ct-projects-icon',
		routerLink: ['/projects'],
		permission: 'roleViewProject'
	},
	{
		label: 'Clients',
		icon: 'ct-clients-icon',
		routerLink: ['/clients'],
		permission: 'roleViewClient'

	},
	{
		label: 'Tasks',
		icon: 'ct-tasks-icon',
		routerLink: ['/tasks'],
		permission: 'roleViewTask'
	},
	{
		label: 'Users',
		icon: 'ct-users-icon',
		routerLink: ['/users'],
		permission: 'roleViewMember'
	},
	{
		label: 'Admin',
		icon: 'ct-admin-icon',
		routerLink: ['/admin'],
		permission: 'roleViewAdminPanel'
	},
	{
		label: 'VSTS Integration',
		icon: 'ct-integration-icon',
		routerLink: ['/vsts-integration'],
		permission: 'roleViewIntegrationPage'
	}
];

@Component({
	selector: 'ct-navigation',
	templateUrl: 'navigation.component.html'
})

export class NavigationComponent implements OnInit, OnDestroy {
	authUser: AuthUser;
	impersonationUser: User;
	items: MenuItem[];
	manageItems: MenuItem[];
	showManageMenu: boolean = false;
	userInfo: User;
	windowWidth: number;

	private subscriptionImpersonation: Subscription;
	private subscriptionUserInfo: Subscription;

	constructor(private authService: AuthService,
	            private aclService: AclService,
	            private auth: AuthGuard,
	            private impersonationService: ImpersonationService,
	            private loadingService: LoadingMaskService,
	            private projectsService: ProjectsService,
	            private usersService: UsersService) {
	}

	ngOnInit() {
		this.authUser = this.authService.authUser;

		this.getUserInfo();
		this.onResize();
		this.updateManageMenuVisibility();

		this.subscriptionUserInfo = this.usersService.onChange.subscribe((userInfo: User) => {
			this.userInfo = userInfo;
		});
		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			this.updateManageMenuVisibility();
		});

		this.items = [
			{
				label: 'Time Tracker',
				icon: 'ct-timetracker-icon',
				routerLink: ['/calendar']
			},
			{
				label: 'Reports',
				icon: 'ct-reports-icon',
				routerLink: ['/reports']
			}
		];
	}

	isMobileView(): boolean {
		return this.windowWidth < 810;
	}

	getUserInfo(): void {
		this.loadingService.addLoading();
		this.usersService.getUserInfo(this.authUser.id).then((userInfo: User) => {
			this.loadingService.removeLoading();
			this.userInfo = userInfo;
		});
	}

	onResize(): void {
		this.windowWidth = window.innerWidth;
	}

	updateManageMenuVisibility(): void {
		this.impersonationUser = this.impersonationService.impersonationUser;
		this.manageItems = FULL_MANAGE_ITEMS;

		if (this.impersonationUser) {
			let isManager = this.impersonationUser.isManager;
			let isAdmin = this.impersonationUser.isAdmin;
			this.showManageMenu = isManager || isAdmin;
			this.authService.isUserAdminOrManager = true;

			if (isManager && !isAdmin) {
				this.manageItems = [FULL_MANAGE_ITEMS[0]];
			}

			return;
		}

		if (this.aclService.isGranted('roleAddProject')) {
			this.showManageMenu = true;
			this.authService.isUserAdminOrManager = true;
		}

		this.loadingService.addLoading();
		this.projectsService.getManagerProjectsCount()
			.finally(() => this.loadingService.removeLoading())
			.subscribe(count => {
				this.showManageMenu = !!count;
				this.authService.isUserAdminOrManager = !!count;
				this.authService.adminOrManagerParameterOnChange.emit();
			});
	}

	signOut(): void {
		this.auth.url = 'calendar';
		this.authService.logout();
	}

	ngOnDestroy() {
		this.subscriptionUserInfo.unsubscribe();
		this.subscriptionImpersonation.unsubscribe();
	}
}
