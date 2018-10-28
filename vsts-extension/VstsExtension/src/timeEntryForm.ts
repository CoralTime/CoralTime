import $ = require("jquery");
import { ITimeEntryFormValues } from "./models/itimeEntry";
import { TimeEntryService } from "./services/timeEntryService";

export class TimeEntryForm {
    private timeEntryService: TimeEntryService;

    private $recordDate = $("#recordDate");
    private $recordTask = $("#recordTask");
    private $recordDescription = $("#recordDescription");
    private $recordSubmit = $("#recordSubmit");
    // private $recordActualWork = $("#recordActualWork");
    // private $recordEstimateWork = $("#recordEstimateWork");

    constructor(private widgetHelpers) {
        this.timeEntryService = new TimeEntryService(widgetHelpers);
        this.initialize();
        this.getTasksForProject();
    }

    initialize(): void {
        this.$recordSubmit.on("click", () => {
            console.log("Saved!");
            this.onSave();
        });
    }

    onSave(): void {
        const timeEntry: ITimeEntryFormValues = {
            date: (this.$recordDate.val() as string),
            description: (this.$recordDescription.val() as string),
            task: (this.$recordTask.val() as string),
        };

        this.timeEntryService.saveTimeEntry(timeEntry)
            .then(
                (data) => console.log("Time entry saved successfully"),
                (error) => console.log("Time entry creation failed"),
            );
    }

    private addOptions(options: string[]): void {
        $.each(options, (index, option) => {
            this.$recordTask.append($("<option/>", {
                text: option,
                value: option,
            }));
        });
    }

    private getTasksForProject(): void {
        this.timeEntryService.getTasksForProject()
            .then((options: { tasks: string[] }) => this.addOptions(options.tasks));
    }
}

const context = VSS.getExtensionContext();
VSS.require("TFS/Dashboards/WidgetHelpers", (WidgetHelpers) => {
    VSS.register(context.publisherId + "." + context.extensionId + ".CoralTimeTracker", () => {
        return new TimeEntryForm(WidgetHelpers);
    });

    VSS.notifyLoadSucceeded();
});
