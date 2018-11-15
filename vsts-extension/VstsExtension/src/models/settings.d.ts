export interface IProjectContext {
    projectId: string;
    userId: string;
}

export interface ISettings {
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
