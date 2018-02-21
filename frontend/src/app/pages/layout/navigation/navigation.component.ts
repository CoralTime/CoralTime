import { AclService } from '../../../core/auth/acl.service';
import { ProjectsService } from '../../../services/projects.service';
import { Subscription } from 'rxjs/Subscription';
import { AuthUser } from '../../../core/auth/auth-user';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';
import { AuthGuard } from '../../../core/auth/auth-guard.service';
import { MenuComponent } from '../../../shared/menu/menu.component';
import { ImpersonationService } from '../../../services/impersonation.service';
import { UserInfoService } from '../../../core/auth/user-info.service';
import { User } from '../../../models/user';

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
		permission: 'roleAssignProjectMember'
	},
	{
		label: 'Clients',
		icon: 'ct-clients-icon',
		routerLink: ['/clients'],
		permission: 'roleAddClient'

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
	}
];

@Component({
	selector: 'ct-navigation',
	templateUrl: 'navigation.component.html'
})

export class NavigationComponent implements OnInit, OnDestroy {
	items: MenuItem[];
	manageItems: MenuItem[];
	authUser: AuthUser;
	showManageMenu: boolean = false;
	isProfileNavMenuOpen: boolean = false;
	isManageMenuOpen: boolean = false;
	userInfo: User;
	windowWidth: number;

	impersonationUser: User;

	private subscriptionUserInfo: Subscription;
	private subscriptionImpersonation: Subscription;

	constructor(private authService: AuthService,
	            private aclService: AclService,
	            private auth: AuthGuard,
	            private impersonationService: ImpersonationService,
	            private projectsService: ProjectsService,
	            private userInfoService: UserInfoService) { }

	ngOnInit() {
		this.authUser = this.authService.getAuthUser();

		this.getUserInfo();
		this.onResize();
		this.updateManageMenuVisibility();

		this.subscriptionUserInfo = this.userInfoService.onChange.subscribe((userInfo: User) => {
			this.userInfo = userInfo;
		});
		this.subscriptionImpersonation = this.impersonationService.onChange.subscribe(() => {
			this.impersonationUser = this.impersonationService.impersonationUser;
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
		return this.windowWidth <= 700;
	}

	getUserInfo(): void {
		this.userInfoService.getUserInfo(this.authUser.id).then((userInfo: User) => {
			this.userInfo =  userInfo;
		});
	}

	onResize(): void {
		this.windowWidth = window.innerWidth;
	}

	updateManageMenuVisibility(): void {
		this.manageItems = FULL_MANAGE_ITEMS;

		if (this.impersonationService.impersonationMember) {
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
			return;
		}

		this.projectsService.getManagerProjectsCount().subscribe(count => {
			this.showManageMenu = !!count;
			this.authService.isUserAdminOrManager = !!count;
			this.authService.adminOrManagerParameterOnChange.emit();
		});
	}

	toggleManageMenu(manageMenu: MenuComponent): void {
		manageMenu.toggleMenu();
		this.isManageMenuOpen = !this.isManageMenuOpen;
	}

	toggleProfileNavMenu(profileNavMenu: MenuComponent): void {
		profileNavMenu.toggleMenu();
		this.isProfileNavMenuOpen = !this.isProfileNavMenuOpen;
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
