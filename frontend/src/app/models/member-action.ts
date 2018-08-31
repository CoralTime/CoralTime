export class MemberAction {
    id: number;
    date: Date;
    changedObject: string;
    entity: string;
    changedFields: ChangedField[];
    action: string;
    entityId: string;
    memberId: number;
    memberFullName: string;

    constructor(data = null) {
        if (data) {
            this.id = data.id;
            this.date = new Date(data.date);
            this.changedObject = data.changedObject
                .replace( /({"|"}|})/g,'')
                .replace(/(","|,")/g, ',  ')
                .replace(/(:"|":"|":)/g, ':');
            this.entity = data.entity
            this.changedFields = JSON.parse(data.changedFields);
            this.action = data.action;
            this.entityId = data.entityId;
            this.memberId = data.memberId;
            this.memberFullName = data.memberFullName;
        }
    }
}

export interface ChangedField {
    field: string;
    oldValue: string;
    newValue: string;
}
