import $ = require("jquery");
import Controls = require("VSS/Controls");
import Combos = require("VSS/Controls/Combos");
import { ITimeEntryFormValues } from "./models/itimeEntry";
import { TimeEntryService } from "./services/timeEntryService";

export class TimeEntryForm {
    private timeEntryService: TimeEntryService;
    private timeEntry: ITimeEntryFormValues = {
        date: "",
        description: "",
        task: "",
    };

    private $date = $(".ct-date-container");
    private $task = $(".ct-task-container");
    private $description = $(".ct-description-container");
    private $actualTimeHours = $(".ct-actual-time-hours");
    private $actualTimeMinutes = $(".ct-actual-time-minutes");

    private $recordSubmit = $("#recordSubmit");

    private taskCombo: any;
    // private $recordActualWork = $("#recordActualWork");
    // private $recordEstimateWork = $("#recordEstimateWork");

    constructor() {
        this.timeEntryService = new TimeEntryService();
        this.initialize();
    }

    initialize(): void {
        this.createTaskSelect();
        this.createDatePicker();
        this.createActualHours();
        this.createActualMinutes();
        this.getTasksForProject();

        console.log(VSS.getConfiguration().witInputs);

        this.$recordSubmit.on("click", () => {
            console.log("Saved!");
            this.onSave();
        });
    }

    onSave(): void {
        // this.timeEntryService.saveTimeEntry(this.timeEntry)
        //     .then(
        //         (data) => console.log("Time entry saved successfully"),
        //         (error) => console.log("Time entry creation failed"),
        //     );
        // this.getWorkItemFormService().then((service) => {
        //     console.log(11, service);
        //     service.setFieldValue("System.Title", "Title set from extension!");
        // });
    }

    private createTaskSelect(tasks?: string[]): void {
        const makeOptions = {
            change: () => {
                this.timeEntry.task = this.taskCombo.getValue();
                if (this.timeEntry.task) {
                    this.taskCombo.setInvalid(true);
                } else {
                    this.taskCombo.setInvalid(false);
                    throw (new Error("The backing field does not have allowed values."));
                }
            },
            enabled: false,
            invalidCss: "ct-invalid",
            validator: (e) => {
                console.log(3, e);
            },
        } as Combos.IComboOptions;

        if (!tasks) {
            this.taskCombo = Controls.create(Combos.Combo, this.$task, makeOptions);
        } else {
            this.taskCombo.setSource(tasks);
            this.taskCombo.setEnabled(true);
        }
    }

    private createDatePicker(): void {
        const dateTimeOptions: Combos.IDateTimeComboOptions = {
            change: () => {
                this.timeEntry.date = dateCombo.getValue();
            },
            dateTimeFormat: "d",
            errorMessage: "Invalid date",
            id: "Date",
            invalidCss: "ct-invalid",
            type: "date-time",
            validator: () => {
            },
            value: new Date().toLocaleDateString("en-US"),
        };

        const dateCombo = Controls.create(Combos.Combo, this.$date, dateTimeOptions);
    }

    private createDescription(): void {
        const makeOptions = {
            change: () => {
            },
            mode: "text",
        } as Combos.IComboOptions;

        const descriptionCombo = Controls.create(Combos.Combo, this.$description, makeOptions);
    }

    private createActualHours(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                console.log(actualCombo.getValue());
                // this.timeEntry.task = taskCombo.getValue();
            },
            invalidCss: "ct-invalid",
            mode: "text",
        } as Combos.IComboOptions;

        const actualCombo = Controls.create(Combos.Combo, this.$actualTimeHours, makeOptions);
    }

    private createActualMinutes(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                console.log(actualCombo.getValue());
                // this.timeEntry.task = taskCombo.getValue();
            },
            invalidCss: "ct-invalid",
            mode: "text",
        } as Combos.IComboOptions;

        const actualCombo = Controls.create(Combos.Combo, this.$actualTimeMinutes, makeOptions);
    }

    private getTasksForProject(): void {
        this.timeEntryService.getTasksForProject()
            .then((options: { tasks: string[] }) => this.createTaskSelect(options.tasks));
    }
}

VSS.register(VSS.getContribution().id, () => {
    return new TimeEntryForm();
});

VSS.notifyLoadSucceeded();
