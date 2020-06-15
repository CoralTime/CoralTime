export class Task {
	color: number;
	description: string;
	id: number;
	isActive: boolean;
	name: string;
	projectId: number = null;

	constructor(data = null) {
		if (data) {
			this.color = data.color;
			this.description = data.description;
			this.id = data.id;
			this.isActive = data.isActive;
			this.name = data.name;
			this.projectId = data.projectId ? data.projectId : null;
		}
	}
}

