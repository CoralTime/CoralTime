import { ProjectRole } from './project-role';

export class UserProject {
	id: number;
	isMemberActive: boolean;
	isProjectActive: boolean;
	isProjectPrivate: boolean;
	memberEmail: string;
	memberId: number;
	memberName: string;
	memberUserName: string;
	projectId: number;
	projectName: string;
	roleName: string;
	roleId: number;
	role: ProjectRole;

	constructor(data = null) {
		if (data) {
			this.id = data.id;
			this.isMemberActive = data.isMemberActive;
			this.isProjectActive = data.isProjectActive;
			this.isProjectPrivate = data.isProjectPrivate;
			this.memberEmail = data.memberEmail;
			this.memberId = data.memberId;
			this.memberName = data.memberName;
			this.memberUserName = data.memberUserName;
			this.projectId = data.projectId;
			this.projectName = data.projectName;
			this.roleName = data.roleName;
			this.roleId = data.roleId;
			this.role = {id: data.roleId, name: data.roleName};
		}
	}
}