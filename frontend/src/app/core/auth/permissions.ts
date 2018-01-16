export const Roles = {
	admin: 1,
	user: 1 << 2
};

export const Permissions = {
	roleAuthenticatedUser: Roles.admin | Roles.user,
	roleAddClient: Roles.admin,
	roleViewTask: Roles.admin,
	roleViewClient: Roles.admin,
	roleAddProject: Roles.admin,
	roleEditProject: Roles.admin,
	roleChangeProjectStatus: Roles.admin,
	roleViewProject: Roles.admin | Roles.user,
	roleAssignProjectManager: Roles.admin,
	roleAssignProjectMember: Roles.admin | Roles.user,
	roleViewMember: Roles.admin,
	roleEditMember: Roles.admin,
	roleDisableMember: Roles.admin
};
