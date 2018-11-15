import $ = require("jquery");
import Controls = require("VSS/Controls");
import Combos = require("VSS/Controls/Combos");
import { ComboO } from "VSS/Controls/Combos";
import { ITask, ITime, ITimeEntryFormValues } from "./models/itimeEntry";
import { TimeEntryService } from "./services/timeEntryService";
import { Notification } from "./utils/notification";

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

    private dateCombo: ComboO<any>;
    private taskCombo: ComboO<any>;
    private actHoursCombo: ComboO<any>;
    private actMinCombo: ComboO<any>;
    private estHoursCombo: ComboO<any>;
    private estMinCombo: ComboO<any>;

    private $submitButton = $("#submitButton");

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
        this.validateForm();

        this.$submitButton.on("click", () => {
            this.onSave();
        });
    }

    onSave(): void {
        this.timeEntryService.saveTimeEntry(this.timeEntry)
            .done(() => {
                Notification.showNotification("Time entry saved successfully.", "success");
            })
            .fail(() => {
                Notification.showNotification("Time entry creation failed.", "error");
            });
    }

    // DATE

    private initDatePicker(): void {
        const dateTimeOptions: Combos.IDateTimeComboOptions = {
            change: () => {
                this.timeEntry.date = this.dateCombo.getText();
                this.validateDate(this.dateCombo);
            },
            dateTimeFormat: "d",
            id: "Date",
            type: "date-time",
            value: new Date().toLocaleDateString("en-US"),
        };

        this.dateCombo = Controls.create(Combos.Combo, $(".ct-date"), dateTimeOptions);
        this.timeEntry.date = this.dateCombo.getText();
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
        this.validateForm();
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
            this.taskCombo.setInvalid(true);
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
        this.validateForm();
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
                this.timeActual.hours = this.actHoursCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(this.actHoursCombo, $actualTimeError, 24, "Actual hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.actHoursCombo = Controls.create(Combos.Combo, $(".ct-actual-time-hours"), makeOptions);
    }

    private initActualMinutes(): void {
        const $actualTimeError = $(".ct-actual-time-container .ct-validation-error").last();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeActual.minutes = this.actMinCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(this.actMinCombo, $actualTimeError, 60, "Actual minutes");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.actMinCombo = Controls.create(Combos.Combo, $(".ct-actual-time-minutes"), makeOptions);
    }

    // ESTIMATED

    private initEstimatedHours(): void {
        const $estimatedTimeError = $(".ct-estimated-time-container .ct-validation-error").first();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.hours = this.estHoursCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(this.estHoursCombo, $estimatedTimeError, 24, "Estimated hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.estHoursCombo = Controls.create(Combos.Combo, $(".ct-estimated-time-hours"), makeOptions);
    }

    private initEstimatedMinutes(): void {
        const $estimatedTimeError = $(".ct-estimated-time-container .ct-validation-error").last();
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.minutes = this.estMinCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(this.estMinCombo, $estimatedTimeError, 60, "Estimated minutes");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.estMinCombo = Controls.create(Combos.Combo, $(".ct-estimated-time-minutes"), makeOptions);
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
        this.validateForm();
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
            }, () => {
                Notification.showGlobalError("Error loading tasks.");
            });
    }

    private validateForm(): void {
        const isDateValid = this.dateCombo.isValid();
        const isTaskValid = this.taskCombo.isValid();
        const isActHoursValid = this.actHoursCombo.isValid();
        const isActMinValid = this.actMinCombo.isValid();
        const isEstHoursValid = this.estHoursCombo.isValid();
        const isEstMinValid = this.estMinCombo.isValid();
        const isFormValid = isDateValid && isTaskValid && isActHoursValid && isActMinValid
            && isEstHoursValid && isEstMinValid;
        this.$submitButton.prop("disabled", !isFormValid);
    }
}

VSS.register(VSS.getContribution().id, () => {
    return new TimeEntryForm();
});

VSS.notifyLoadSucceeded();
