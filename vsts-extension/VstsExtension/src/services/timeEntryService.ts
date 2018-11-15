import { IProjectContext, ISettings, IUserSettings, IWorkItemOptions } from "../models/settings";
import { ITimeEntry, ITimeEntryFormValues } from "../models/timeEntry";
import { ConfigurationService } from "./configurationService";

export class TimeEntryService {
    private configService: ConfigurationService;
    private extensionSettings: ISettings;
    private projectContext: IProjectContext;

    constructor() {
        this.configService = new ConfigurationService();
        this.configService.accessTokenPromise.then(() => {
            this.loadConfiguration();
        });
    }

    getTasksForProject(): PromiseLike<any> {
        return this.configService.accessTokenPromise.then(() => $.get(this.extensionSettings.siteUrl
            + "/api/v1/VSTS/Tasks?ProjectId=" + this.projectContext.projectId));
    }

    getTimeEntries(): PromiseLike<any> {
        return Promise.all([this.configService.accessTokenPromise, this.configService.workItemFormPromise])
            .then(() => $.get(this.extensionSettings.siteUrl + "/api/v1/VSTS/TimeEntries?ProjectId="
                + this.projectContext.projectId + "&WorkItemId=" + this.getWorkItemOptions()["System.Id"]));
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

        return $.post(this.extensionSettings.siteUrl + "/api/v1/VSTS/TimeEntries", JSON.stringify(timeEntry));
    }

    private getUserSettings(): IUserSettings {
        return this.configService.getUserSettings();
    }

    private getWorkItemOptions(): IWorkItemOptions {
        return this.configService.getWorkItemOptions();
    }

    private formatDescription(description: string): string {
        const des = description ? " - " + description : "";
        return this.getWorkItemOptions()["System.WorkItemType"] + " #" + this.getWorkItemOptions()["System.Id"]
            + " " + this.getWorkItemOptions()["System.Title"] + des;
    }

    private formatDate(date: string): string {
        return date.split("/").join("-");
    }

    private loadConfiguration(): void {
        this.extensionSettings = this.configService.getExtensionSettings();
        this.projectContext = this.configService.getExtensionContext();
    }
}
