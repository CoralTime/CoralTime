export class VstsProjectConnection {
	id: number;
	membersCount: number;
	projectId: number;
	projectName: string;
	vstsProjectId: number;
	vstsProjectName: string;
	vstsCompanyUrl: string;
	vstsPat: string;


	constructor(data?: any) {
		this.id = data && data.id;
		this.membersCount = data && data.membersCount;
		this.projectId = data && data.projectId;
		this.projectName = data && data.projectName;
		this.vstsProjectId = data && data.vstsProjectId;
		this.vstsProjectName = data && data.vstsProjectName;
		this.vstsCompanyUrl = data && data.vstsCompanyUrl;
		this.vstsPat = data && data.vstsPat;
	}
}
