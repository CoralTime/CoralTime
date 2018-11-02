import { IProjectContext } from "../models/iprojectContext";
import { ITimeEntry, ITimeEntryFormValues } from "../models/itimeEntry";
import { ConfigurationService } from "./configurationService";

export class TimeEntryService {
    private configService: ConfigurationService;
    private projectContext: IProjectContext;
    private extensionSettings: ISettings;

    constructor() {
        this.configService = new ConfigurationService();
        this.configService.accessTokenPromise.then(() => {
            this.loadConfiguration();
        });
    }

    getTasksForProject(): PromiseLike<any> {
        return this.configService.accessTokenPromise.then(() => {
            return $.ajax({
                dataType: "json",
                headers: {
                    "Content-Type": "application/json",
                    "VSTSToken": "Bearer " + this.configService.getAccessToken(),
                },
                type: "get",
                url: this.extensionSettings.siteUrl + "/api/v1/VSTS/Tasks?ProjectName=" + this.projectContext.projectName,
            });
        });
    }

    saveTimeEntry(values: ITimeEntryFormValues): Promise<any> {
        const timeEntry: ITimeEntry = {
            date: values.date,
            description: values.description,
            task: values.task,
            userEmail: this.projectContext.userEmail,
            userName: this.projectContext.userName,
            vstsProjectId: this.projectContext.projectId,
            vstsProjectName: this.projectContext.projectName,
        };

        return $.ajax({
            data: timeEntry,
            headers: {
                "Authorization": "Bearer " + this.configService.getAccessToken(),
                "Content-Type": "application/json",
            },
            type: "post",
            url: this.extensionSettings.siteUrl + "/api/v1/timeentires/VSTS",
        });
    }

    private loadConfiguration(): void {
        this.projectContext = this.configService.getExtensionContext();
        this.extensionSettings = this.configService.getSettings();
    }
}
