export interface ITimeEntry {
    // actualTime: number,
    date: string;
    description: string;
    // estimatedTime: number,
    task: string;
    userEmail: string;
    userName: string;
    vstsProjectId: string;
    vstsProjectName: string;
}

export interface ITimeEntryFormValues {
    date: string;
    description: string;
    task: string;
}
