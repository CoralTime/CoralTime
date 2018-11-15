export interface ITimeEntry {
    date: string;
    description: string;
    memberId: number;
    projectId: number;
    taskTypesId: number;
    timeOptions: ITimeOptions;
    timeValues: ITimeValues;
    workItemId: string;
}

export interface ITimeOptions {
    timeTimerStart: number;
    isFromToShow: boolean;
}

export interface ITimeValues {
    timeFrom: number;
    timeTo: number;
    timeActual: number;
    timeEstimated: number;
}

export interface ITask {
    id: number;
    name: string;
}

export interface ITimeEntryFormValues {
    date: string;
    description: string;
    taskId: number;
    timeActual: number;
    timeEstimated: number;
}

export interface ITime {
    hours: number;
    minutes: number;
}

export interface ITimeEntryRow {
    date: string;
    description: string;
    memberId?: number;
    memberName: string;
    projectId?: number;
    taskId?: number;
    taskName: string;
    timeOptions?: ITimeOptions;
    timeValues: ITimeValues;
    workItemId?: string;
}
