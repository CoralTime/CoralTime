import $ = require("jquery");
import { ConfigurationService } from "./configurationService";
import { IProjectContext } from "./models/iprojectContext";

const OPTIONS = [
    {
        label: "Coding",
        value: "1",
    },
    {
        label: "Reading",
        value: "2",
    },
    {
        label: "Test",
        value: "3",
    },
];

export class TimeEntryService {

    private $recordDate = $("#recordDate");
    private $recordTask = $("#recordTask");
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
                console.log("Error get tasks for CoralTime", data);
                this.addOptions();
            },
            headers: {
                "Content-Type": "application/json",
                "VSTSToken": "Bearer " + this.config.getAccessToken(),
            },
            success: (data) => {
                console.log("Time entry saved successfully", data);
            },
            type: "get",
            url: this.extensionSettings.siteUrl + "/api/v1/VSTS/Tasks?ProjectName=" + this.projectContext.projectName,
        });
    }

    public onSave(): void {
        this.loadConfiguration();

        const timeEntry = {
            // actualTime: (this.$recordActualWork.val() as number),
            date: (this.$recordDate.val() as string),
            description: (this.$recordDescription.val() as string),
            // estimatedTime: (this.$recordEstimateWork.val() as number),
            task: (this.$recordTask.val() as string),
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
            url: this.extensionSettings.siteUrl + "/api/v1/timeentires/VSTS",
        });
    }

    private loadConfiguration(): void {
        this.projectContext = this.config.getExtensionContext();
        this.extensionSettings = this.config.getSettings();
    }

    private addOptions(): void {
        $.each(OPTIONS, (index, itemData) => {
            this.$recordTask.append($("<option/>", {
                text: itemData.label,
                value: itemData.value,
            }));
        });
    }
}

const context = VSS.getExtensionContext();
VSS.require("TFS/Dashboards/WidgetHelpers", (WidgetHelpers) => {
    VSS.register(context.publisherId + "." + context.extensionId + ".CoralTimeTracker", () => {
        return new TimeEntryService(new ConfigurationService(WidgetHelpers));
    });
    VSS.notifyLoadSucceeded();
});
