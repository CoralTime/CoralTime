import { Configuration } from "./configuration";
import { IProjectContext } from "./iprojectContext";

export class TimeEntryService {

    private $recordDate = $("#recordDate");
    private $recordTaskType = $("#recordTaskType");
    private $recordDescription = $("#recordDescription");
    private $recordSubmit = $("#recordSubmit");
    // private $recordActualWork = $("#recordActualWork");
    // private $recordEstimateWork = $("#recordEstimateWork");
    private config: Configuration = new Configuration();
    private projectContext: IProjectContext;
    private extensionSettings: ISettings;

    constructor() {
        this.initialize();
    }

    public initialize(): void {
        this.$recordSubmit.click(() => {
            console.log("Saved!");
            this.onSave();
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
