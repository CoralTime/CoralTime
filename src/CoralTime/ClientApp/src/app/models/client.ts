export class Client {
	id: number;
	name: string;
	email: string;
	description: string;
	isActive: boolean;
	projectsCount: number;

	constructor(data = null) {
		if (data) {
			this.id = data.id;
			this.name = data.name;
			this.email = data.email;
			this.description = data.description;
			this.isActive = data.isActive;
			this.projectsCount = data.projectsCount;
		}
	}
}
