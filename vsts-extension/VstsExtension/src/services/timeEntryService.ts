import { IProjectContext } from "../models/iprojectContext";
import { ISettings } from "../models/isettings";
import { ISystemOptions } from "../models/isystemOptions";
import { ITimeEntry, ITimeEntryFormValues } from "../models/itimeEntry";
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
        return this.configService.accessTokenPromise.then(() => {
            return $.get(this.extensionSettings.siteUrl + "/api/v1/VSTS/Tasks?ProjectId=" + this.projectContext.projectId);
        });
    }

    saveTimeEntry(values: ITimeEntryFormValues): JQuery.jqXHR {
        const description = values.description ? " - " + values.description : "";
        const timeEntry: ITimeEntry = {
            date: values.date,
            description: this.getSystemOptions()["System.WorkItemType"] + " #" + this.getSystemOptions()["System.Id"]
                + " " + this.getSystemOptions()["System.Title"] + description,
            memberId: this.configService.getUserSettings().memberId,
            projectId: this.configService.getUserSettings().projectId,
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
            workItemId: String(this.getSystemOptions()["System.Id"]),
        };

        return $.post(this.extensionSettings.siteUrl + "/api/v1/VSTS/TimeEntries", JSON.stringify(timeEntry));
    }

    private getSystemOptions(): ISystemOptions {
        return this.configService.getSystemOptions();
    }

    private loadConfiguration(): void {
        this.extensionSettings = this.configService.getExtensionSettings();
        this.projectContext = this.configService.getExtensionContext();
    }
}
