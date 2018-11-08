import $ = require("jquery");
import Controls = require("VSS/Controls");
import Combos = require("VSS/Controls/Combos");
import { ComboO } from "VSS/Controls/Combos";
import { ITask, ITime, ITimeEntryFormValues } from "./models/itimeEntry";
import { TimeEntryService } from "./services/timeEntryService";

export class TimeEntryForm {
    private tasksOptions: ITask[];
    private timeEntryService: TimeEntryService;
    private timeEntry: ITimeEntryFormValues = {
        date: "",
        description: "",
        taskId: 0,
        timeActual: 0,
        timeEstimated: 0,
    };
    private timeActual: ITime = {hours: 0, minutes: 0};
    private timeEstimated: ITime = {hours: 0, minutes: 0};

    private taskCombo: ComboO<any>;

    constructor() {
        this.timeEntryService = new TimeEntryService();
        this.initialize();
    }

    initialize(): void {
        this.initDatePicker();
        this.initTaskSelect();
        this.initDescription();
        this.initActualHours();
        this.initActualMinutes();
        this.initEstimatedHours();
        this.initEstimatedMinutes();
        this.getTasksForProject();

        console.log(VSS.getConfiguration().witInputs);

        $("#submitButton").on("click", () => {
            this.onSave();
        });
    }

    onSave(): void {
        this.timeEntryService.saveTimeEntry(this.timeEntry)
            .done(() => {
                console.log("Time entry saved successfully");
            })
            .fail(() => {
                console.log("Time entry creation failed");
            });
    }

    // DATE

    private initDatePicker(): void {
        const dateTimeOptions: Combos.IDateTimeComboOptions = {
            change: () => {
                this.timeEntry.date = dateCombo.getText();
                this.validateDate(dateCombo);
            },
            dateTimeFormat: "d",
            id: "Date",
            type: "date-time",
            value: new Date().toLocaleDateString("en-US"),
        };

        const dateCombo = Controls.create(Combos.Combo, $(".ct-date"), dateTimeOptions);
        this.timeEntry.date = dateCombo.getText();
    }

    private validateDate(dateCombo: ComboO<any>): void {
        const $dateError = $(".ct-date-container .ct-validation-error");
        let errorMessage: string = "";

        if (!dateCombo.getValue()) {
            errorMessage = "Invalid date.";
        }

        dateCombo.setInvalid(!!errorMessage);
        $dateError.text(errorMessage);
        $dateError.css("visibility", !!errorMessage ? "visible" : "hidden");
    }

    // TASK

    private initTaskSelect(tasks?: string[]): void {
        const makeOptions = {
            change: () => {
                this.timeEntry.taskId = this.getTaskIdByName(this.taskCombo.getValue());
                this.validateTask(this.taskCombo);
            },
            enabled: false,
        } as Combos.IComboOptions;

        if (!tasks) {
            this.taskCombo = Controls.create(Combos.Combo, $(".ct-task"), makeOptions);
        } else {
            this.taskCombo.setSource(tasks);
            this.taskCombo.setEnabled(true);
        }
    }

    private getTaskNames(): string[] {
        return this.tasksOptions.map((x) => x.name);
    }

    private getTaskIdByName(name: string): number {
        const task: ITask = this.tasksOptions.filter((task: ITask) => task.name === name)[0];
        return task ? task.id : null;
    }

    private validateTask(taskCombo: ComboO<any>): void {
        const $taskError = $(".ct-task-container .ct-validation-error");
        const value: string = taskCombo.getValue();
        let errorMessage: string = "";

        if (!value) {
            errorMessage = "Task can't be empty.";
        }

        if (this.getTaskNames().indexOf(value) === -1) {
            errorMessage = "The backing field does not have allowed values.";
        }

        taskCombo.setInvalid(!!errorMessage);
        $taskError.text(errorMessage);
        $taskError.css("visibility", errorMessage ? "visible" : "hidden");
    }

    // DESCRIPTION

    private initDescription(): void {
        const $description = $("#description");
        $description.on("change", () => {
            this.timeEntry.description = String($description.val());
        });
    }

    // ACTUAL

    private initActualHours(): void {
        const $actualTimeError = $(".ct-actual-time-container .ct-validation-error").first();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeActual.hours = actualCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(actualCombo, $actualTimeError, 24, "Actual hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        const actualCombo = Controls.create(Combos.Combo, $(".ct-actual-time-hours"), makeOptions);
    }

    private initActualMinutes(): void {
        const $actualTimeError = $(".ct-actual-time-container .ct-validation-error").last();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeActual.minutes = actualCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(actualCombo, $actualTimeError, 60, "Actual minutes");
            },
            invalidCss: "ct-invalid",
            mode: "text",
        } as Combos.IComboOptions;

        const actualCombo = Controls.create(Combos.Combo, $(".ct-actual-time-minutes"), makeOptions);
    }

    // ESTIMATED

    private initEstimatedHours(): void {
        const $estimatedTimeError = $(".ct-estimated-time-container .ct-validation-error").first();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.hours = estimatedCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(estimatedCombo, $estimatedTimeError, 24, "Estimated hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        const estimatedCombo = Controls.create(Combos.Combo, $(".ct-estimated-time-hours"), makeOptions);
    }

    private initEstimatedMinutes(): void {
        const $estimatedTimeError = $(".ct-estimated-time-container .ct-validation-error").last();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.minutes = estimatedCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(estimatedCombo, $estimatedTimeError, 60, "Estimated minutes");
            },
            mode: "text",
        } as Combos.IComboOptions;

        const estimatedCombo = Controls.create(Combos.Combo, $(".ct-estimated-time-minutes"), makeOptions);
    }

    private validateTime(timeCombo: ComboO<any>, errorContainer: JQuery, maxValue: number, label: string): void {
        const value: number = Number(timeCombo.getValue());
        let errorMessage: string = "";

        if (isNaN(value)) {
            errorMessage = "Invalid format of " + label + ".";
        }

        if (value < 0) {
            errorMessage = label + " can't be less than 0.";
        }

        if (value > maxValue) {
            errorMessage = label + " can't be more than " + maxValue + ".";
        }

        timeCombo.setInvalid(!!errorMessage);
        errorContainer.text(errorMessage);
        errorContainer.css("visibility", errorMessage ? "visible" : "hidden");
    }

    private convertTimeToNumber(time: ITime): number {
        return time.hours * 3600 + time.minutes * 60;
    }

    // GENERAL

    private getTasksForProject(): void {
        this.timeEntryService.getTasksForProject()
            .then((options: ITask[]) => {
                this.tasksOptions = options;
                this.initTaskSelect(this.getTaskNames());
            });
    }
}

VSS.register(VSS.getContribution().id, () => {
    return new TimeEntryForm();
});

VSS.notifyLoadSucceeded();
