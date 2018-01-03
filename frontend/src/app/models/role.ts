export class Role {
	id: number;
	name: string;

	constructor(data = null) {
		if (data) {
			this.id = data.id;
			this.name = data.name;
		}
	}
}

