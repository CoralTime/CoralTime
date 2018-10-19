import { ConfigurationService } from "./configurationService";
import { IProjectContext } from "./models/iprojectContext";

export class TimeEntryService {

    private $recordDate = $("#recordDate");
    private $recordTaskType = $("#recordTaskType");
    private $recordDescription = $("#recordDescription");
    private $recordSubmit = $("#recordSubmit");
    // private $recordActualWork = $("#recordActualWork");
    // private $recordEstimateWork = $("#recordEstimateWork");
    private projectContext: IProjectContext;
    private extensionSettings: ISettings;

    constructor(private config: ConfigurationService) {
        this.initialize();
        this.config.accessTokenPromise.then(() => {
            this.getTasksForProject();
        });
    }

    public initialize(): void {
        this.loadConfiguration();
        this.$recordSubmit.click(() => {
            console.log("Saved!");
            this.onSave();
        });
    }

    public getTasksForProject() {
        $.ajax({
            dataType: "json",
            error: (data) => {
                alert("Error get tasks for CoralTime");
            },
            headers: {
                "Content-Type": "application/json",
                "VSTSToken": "Bearer " + this.config.getAccessToken(),
            },
            success: (data) => {
                console.log(data);
                alert("Time entry saved successfully");
            },
            type: "get",
            url: this.extensionSettings.siteUrl + "/api/v1/vsts/tasks?ProjectName=" + this.projectContext.projectName,
        });
    }

    public onSave(): void {
        this.loadConfiguration();

        const timeEntry = {
            // actualTime: (this.$recordActualWork.val() as number),
            date: (this.$recordDate.val() as string),
            description: (this.$recordDescription.val() as string),
            // estimatedTime: (this.$recordEstimateWork.val() as number),
            task: (this.$recordTaskType.val() as string),
            userEmail: (this.projectContext.userEmail as string),
            userName: (this.projectContext.userName as string),
            vstsProjectId: (this.projectContext.projectId as string),
            vstsProjectName: (this.projectContext.projectName as string),
        };

        $.ajax({
            data: timeEntry,
            error: (data) => {
                alert("Time entry creation failed");
            },
            headers: {
                "Authorization": "Bearer " + this.config.getAccessToken(),
                "Content-Type": "application/json",
            },
            success: (data) => {
                alert("Time entry saved successfully");
            },
            type: "post",
            url: this.extensionSettings.siteUrl + "/api/v1/timeentires/vsts",
        });
    }

    private loadConfiguration(): void {
        this.projectContext = this.config.getExtensionContext();
        this.extensionSettings = this.config.getSettings();
    }
}
