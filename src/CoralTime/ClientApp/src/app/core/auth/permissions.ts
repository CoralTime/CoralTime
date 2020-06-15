// Each role - prime number
export const Roles = {
	admin: 1,
	manager: 2,
	user: 3
};

export const Permissions = {
	roleAuthenticatedUser: Roles.admin * Roles.manager * Roles.user,
	roleViewProject: Roles.admin * Roles.manager,
	roleAddProject: Roles.admin * Roles.manager,
	roleEditProject: Roles.admin * Roles.manager,
	roleChangeProjectStatus: Roles.admin * Roles.manager,
	roleAssignProjectManager: Roles.admin,
	roleAssignProjectMember: Roles.admin * Roles.manager,
	roleViewClient: Roles.admin,
	roleAddClient: Roles.admin,
	roleEditClient: Roles.admin,
	roleViewTask: Roles.admin,
	roleAddTask: Roles.admin,
	roleEditTask: Roles.admin,
	roleViewMember: Roles.admin,
	roleAddMember: Roles.admin,
	roleEditMember: Roles.admin,
	roleViewAdminPanel: Roles.admin,
	roleViewIntegrationPage: Roles.admin
};
