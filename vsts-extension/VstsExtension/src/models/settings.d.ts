export interface IProjectContext {
    projectId: string;
    teamId: string;
    userId: string;
}

export interface ISettings {
    id: string;
    siteUrl: string;
}

export interface IUserSettings {
    expirationDate: number;
    memberId: number;
    projectId: number;
}

export interface IWorkItemOptions {
    "System.Id": number;
    "System.Title": string;
    "System.WorkItemType": string;
}
