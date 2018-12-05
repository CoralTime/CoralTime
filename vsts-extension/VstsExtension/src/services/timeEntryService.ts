import { ITask, ITimeEntry, ITimeEntryFormValues, ITimeEntryRow } from "../models/timeEntry";
import { ConfigurationService } from "./configurationService";

export class TimeEntryService extends ConfigurationService {
    constructor() {
        super();
    }

    getTasksForProject(): Promise<ITask[]> {
        return Promise.all([this.accessTokenPromise, this.extensionSettingsPromise])
            .then(() => $.get(this.getExtensionSettings().siteUrl
                + "/api/v1/VSTS/Tasks?ProjectId=" + this.getProjectContext().projectId));
    }

    getTimeEntries(): Promise<ITimeEntryRow[]> {
        return Promise.all([this.accessTokenPromise, this.extensionSettingsPromise, this.workItemFormPromise])
            .then(() => $.get(this.getExtensionSettings().siteUrl + "/api/v1/VSTS/TimeEntries?ProjectId="
                + this.getProjectContext().projectId + "&WorkItemId=" + this.getWorkItemOptions()["System.Id"]));
    }

    saveTimeEntry(values: ITimeEntryFormValues): JQuery.jqXHR {
        const timeEntry: ITimeEntry = {
            date: this.formatDate(values.date),
            description: this.formatDescription(values.description),
            memberId: this.getUserSettings().memberId,
            projectId: this.getUserSettings().projectId,
            taskTypesId: values.taskId,
            timeOptions: {
                isFromToShow: false,
                timeTimerStart: 0,
            },
            timeValues: {
                timeActual: values.timeActual,
                timeEstimated: values.timeEstimated,
                timeFrom: null,
                timeTo: null,
            },
            workItemId: String(this.getWorkItemOptions()["System.Id"]),
        };

        return $.post(this.getExtensionSettings().siteUrl + "/api/v1/VSTS/TimeEntries", JSON.stringify(timeEntry));
    }

    private formatDescription(description: string): string {
        const des = description ? " - " + description : "";
        const link = "[" + this.getWorkItemOptions()["System.WorkItemType"] + " #"
            + this.getWorkItemOptions()["System.Id"] + "](" + document.referrer + ")";
        return link + " " + this.getWorkItemOptions()["System.Title"] + des;
    }

    private formatDate(date: string): string {
        return date.split("/").join("-");
    }
}
