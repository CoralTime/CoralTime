// tslint:disable-next-line
/// <reference path="isettings.d.ts"/>

// import TelemetryClient = require("scripts/TelemetryClient");

import Configuration = require("./configuration");
import { IProjectContext } from "./iprojectContext";

export class TiemEntryService {

    private $recordTaskType = $("recordTaskType-input");
    private $recordDate = $("#recordDate-input");
    private $recordActualWork = $("#recordActualWork-input");
    private $recordEstimateWork = $("#recordEstimateWork-input");
    private config: Configuration.Configuration;
    private projectContext: IProjectContext;
    private extentionSettings: ISettings;

    public onSave() {
        this.loadConfiguration();

        const timeEntry = {
            actualTime: (this.$recordActualWork.val() as number),
            date: (this.$recordDate.val() as string),
            estimatedTime: (this.$recordEstimateWork.val() as number),
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
            url: this.extentionSettings.siteUrl + "/api/v1/timeentires/vsts",
        });
    }

    private loadConfiguration() {
        this.projectContext = this.config.getExtensionContext();
        this.extentionSettings = this.config.getSettings();
    }
}
