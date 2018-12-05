import $ = require("jquery");
import Controls = require("VSS/Controls");
import Combos = require("VSS/Controls/Combos");
import { ComboO } from "VSS/Controls/Combos";
import { Grid } from "VSS/Controls/Grids";
import Grids = require("VSS/Controls/Grids");
import { IGridColumn, IGridRowInfo } from "VSS/Controls/Grids";
import { ISettings } from "./models/settings";
import { ITask, ITime, ITimeEntryFormValues, ITimeEntryRow, ITimeValues } from "./models/timeEntry";
import { TimeEntryService } from "./services/timeEntryService";
import { Notification } from "./utils/notification";

export class TimeEntryForm {
    siteUrl: string;
    timeEntry: ITimeEntryFormValues = {
        date: "",
        description: "",
        taskId: 0,
        timeActual: 0,
        timeEstimated: 0,
    };
    timeActual: ITime = {hours: 0, minutes: 0};
    timeEstimated: ITime = {hours: 0, minutes: 0};

    private isConfigurationOpened: boolean;
    private tasksOptions: ITask[];
    private timeEntryRows: ITimeEntryRow[];
    private timeEntryService: TimeEntryService;

    // FORM
    private dateCombo: ComboO<any>;
    private taskCombo: ComboO<any>;
    private actHoursCombo: ComboO<any>;
    private actMinCombo: ComboO<any>;
    private estHoursCombo: ComboO<any>;
    private estMinCombo: ComboO<any>;
    private urlCombo: ComboO<any>;
    private gridControl: Grid;

    // BUTTONS
    private $configurationPage = $(".ct-coraltime-configuration");
    private $coraltimeFormPage = $(".ct-coraltime");
    private $description = $("#description");
    private $submitButton = $("#submitButton");
    private $submitButton2 = $("#submitButton2");
    private $toggleButton = $(".ct-toggle-page-button");

    // ERRORS
    private $actualHoursError = $(".ct-actual-time-container .ct-validation-error").first();
    private $actualMinutesError = this.$actualHoursError.next();
    private $estimatedHoursError = $(".ct-estimated-time-container .ct-validation-error").first();
    private $estimatedMinutesError = this.$estimatedHoursError.next();
    private $dateError = $(".ct-date-container .ct-validation-error");
    private $taskError = $(".ct-task-container .ct-validation-error");
    private $urlError = $(".ct-url-container .ct-validation-error");

    constructor() {
        this.timeEntryService = new TimeEntryService();
        this.initConfigurationForm();
        this.initCoralTimeForm();
        this.checkExtensionSettings();
        $(".ct-toggle-page-button").on("click", () => {
            this.togglePage(!this.isConfigurationOpened);
        });
    }

    checkExtensionSettings(): void {
        Promise.all([this.timeEntryService.extensionSettingsPromise, this.timeEntryService.userDetailsPromise])
            .then(([settings, isUserTeamAdmin]) => {
                if (!settings || !settings.siteUrl) {
                    if (isUserTeamAdmin) {
                        this.togglePage(true);
                    } else {
                        Notification.showGlobalError("Settings is not defined.", "form");
                    }
                    return;
                }

                this.getTasksForProject();
                this.getTimeEntries();
                this.urlCombo.setInputText(this.getSiteUrl(settings), true);
            });
    }

    initCoralTimeForm(): void {
        this.initDatePicker();
        this.initTaskSelect();
        this.initDescription();
        this.initActualHours();
        this.initActualMinutes();
        this.initEstimatedHours();
        this.initEstimatedMinutes();
        this.validateForm();

        this.$submitButton2.on("click", () => {
            this.saveForm();
        });
    }

    initConfigurationForm(): void {
        this.initSiteUrlInput();
        this.$submitButton.on("click", () => this.saveUrl());
    }

    saveForm(): void {
        this.timeEntryService.saveTimeEntry(this.timeEntry)
            .done(() => {
                Notification.showNotification("Time entry saved successfully.", "success", "form");
                this.resetForm();
                this.getTimeEntries();
            })
            .fail(() => {
                Notification.showNotification("Time entry creation failed.", "error", "form");
            });
    }

    resetForm(): void {
        this.$description.val("");
        this.actHoursCombo.setText("");
        this.actMinCombo.setText("");
        this.estHoursCombo.setText("");
        this.estMinCombo.setText("");
        this.timeEntry.description = "";
        this.timeEntry.timeActual = 0;
        this.timeEntry.timeEstimated = 0;
    }

    saveUrl(): void {
        if (!this.timeEntryService.isUserTeamAdmin()) {
            Notification.showNotification("You haven't access to change configuration.", "error", "configuration");
            return;
        }
        this.timeEntryService.setExtensionSettings(this.siteUrl).then(() => {
            Notification.showNotification("Site url changed.", "success", "configuration");

            setTimeout(() => {
                Notification.removeGlobalErrors();
                this.getTasksForProject();
                this.getTimeEntries();
                this.timeEntryService.updateUserSettings();
                this.togglePage(false);
            }, 5000);
        });
    }

    togglePage(isConfigurationPage: boolean): void {
        if (!this.timeEntryService.isUserTeamAdmin()) {
            return;
        }
        this.isConfigurationOpened = isConfigurationPage;
        this.$toggleButton.toggleClass("ct-configuration-opened", isConfigurationPage);
        if (isConfigurationPage) {
            this.$configurationPage.show();
            this.$coraltimeFormPage.hide();
        } else {
            this.$configurationPage.hide();
            this.$coraltimeFormPage.css("display", "flex");
        }
    }

    private validateForm(): void {
        const isDateValid = this.dateCombo.isValid();
        const isTaskValid = this.taskCombo.isValid();
        const isActHoursValid = this.actHoursCombo.isValid();
        const isActMinValid = this.actMinCombo.isValid();
        const isEstHoursValid = this.estHoursCombo.isValid();
        const isEstMinValid = this.estMinCombo.isValid();
        const isTimeValuesValid = !!this.timeEntry.timeActual || !!this.timeEntry.timeEstimated;
        const isFormValid = isDateValid && isTaskValid && isActHoursValid && isActMinValid
            && isEstHoursValid && isEstMinValid && isTimeValuesValid;
        this.$submitButton2.prop("disabled", !isFormValid);
    }

    // SITE URL

    private initSiteUrlInput(): void {
        const urlOptions: Combos.IDateTimeComboOptions = {
            change: () => {
                this.siteUrl = this.urlCombo.getValue();
                this.validateUrl(this.urlCombo);
            },
            mode: "text",
            value: this.siteUrl,
        };

        this.urlCombo = Controls.create(Combos.Combo, $(".ct-url"), urlOptions);
        this.validateUrl(this.urlCombo);
    }

    private getSiteUrl(settings: ISettings): string {
        return settings && settings.siteUrl.substr(8);
    }

    private validateUrl(urlCombo: ComboO<any>): void {
        let errorMessage: string = "";

        if (!urlCombo.getValue()) {
            errorMessage = "Site url is required.";
        }

        if (!/^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$/.test(urlCombo.getValue())) {
            errorMessage = "Invalid url.";
        }

        urlCombo.setInvalid(!!errorMessage);
        this.$urlError.text(errorMessage);
        this.$urlError.css("visibility", !!errorMessage ? "visible" : "hidden");
        this.$submitButton.prop("disabled", !!errorMessage);
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
        let errorMessage: string = "";

        if (!dateCombo.getValue()) {
            errorMessage = "Invalid date.";
        }

        dateCombo.setInvalid(!!errorMessage);
        this.$dateError.text(errorMessage);
        this.$dateError.css("visibility", !!errorMessage ? "visible" : "hidden");
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

    private getTasksForProject(): void {
        this.timeEntryService.getTasksForProject()
            .then((options: ITask[]) => {
                this.tasksOptions = options;
                this.initTaskSelect(this.getTaskNames());
            }, () => {
                Notification.showGlobalError("Error loading tasks.", "form");
            });
    }

    private getTaskNames(): string[] {
        return this.tasksOptions.map((x) => x.name);
    }

    private getTaskIdByName(name: string): number {
        const task: ITask = this.tasksOptions.filter((task: ITask) => task.name === name)[0];
        return task ? task.id : null;
    }

    private validateTask(taskCombo: ComboO<any>): void {
        const value: string = taskCombo.getValue();
        let errorMessage: string = "";

        if (!value) {
            errorMessage = "Task can't be empty.";
        }

        if (this.getTaskNames().indexOf(value) === -1) {
            errorMessage = "The backing field does not have allowed values.";
        }

        taskCombo.setInvalid(!!errorMessage);
        this.$taskError.text(errorMessage);
        this.$taskError.css("visibility", errorMessage ? "visible" : "hidden");
        this.validateForm();
    }

    // DESCRIPTION

    private initDescription(): void {
        this.$description.on("change", () => {
            this.timeEntry.description = String(this.$description.val());
        });
    }

    // ACTUAL

    private initActualHours(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeActual.hours = this.actHoursCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(this.actHoursCombo, this.$actualHoursError, 24, "Actual hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.actHoursCombo = Controls.create(Combos.Combo, $(".ct-actual-time-hours"), makeOptions);
    }

    private initActualMinutes(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeActual.minutes = this.actMinCombo.getValue();
                this.timeEntry.timeActual = this.convertTimeToNumber(this.timeActual);
                this.validateTime(this.actMinCombo, this.$actualMinutesError, 60, "Actual minutes");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.actMinCombo = Controls.create(Combos.Combo, $(".ct-actual-time-minutes"), makeOptions);
    }

    // ESTIMATED

    private initEstimatedHours(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.hours = this.estHoursCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(this.estHoursCombo, this.$estimatedHoursError, 24, "Estimated hours");
            },
            mode: "text",
        } as Combos.IComboOptions;

        this.estHoursCombo = Controls.create(Combos.Combo, $(".ct-estimated-time-hours"), makeOptions);
    }

    private initEstimatedMinutes(): void {
        const makeOptions = {
            autoComplete: false,
            change: () => {
                this.timeEstimated.minutes = this.estMinCombo.getValue();
                this.timeEntry.timeEstimated = this.convertTimeToNumber(this.timeEstimated);
                this.validateTime(this.estMinCombo, this.$estimatedMinutesError, 60, "Estimated minutes");
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

    private convertNumberToTime(time: number): ITime {
        const hours = Math.floor(time / 3600);
        const minutes = Math.floor(time / 60) - hours * 60;
        return {hours, minutes};
    }

    // GRID

    private initGrid(): void {
        const container = $(".ct-grid");
        const gridOptions: Grids.IGridOptions = {
            columns: [
                {
                    index: "memberName",
                    text: "User",
                    width: 150,
                },
                {
                    getCellContents: (
                        rowInfo: IGridRowInfo,
                        dataIndex: number,
                        expandedState: number,
                        level: number,
                        column: IGridColumn,
                        indentIndex: number,
                        columnOrder: number) => {
                        const date: string = this.timeEntryRows[dataIndex][column.index];

                        return $("<div class='grid-cell'/>")
                            .width(column.width)
                            .text(new Date(date).toLocaleDateString("en-US"));
                    },
                    index: "date",
                    text: "Date",
                    width: 100,
                },
                {
                    index: "taskName",
                    text: "Task",
                    width: 130,
                },
                {
                    canSortBy: false,
                    index: "description",
                    text: "Description",
                    width: 500,
                },
                {
                    canSortBy: false,
                    getCellContents: (
                        rowInfo: IGridRowInfo,
                        dataIndex: number,
                        expandedState: number,
                        level: number,
                        column: IGridColumn,
                        indentIndex: number,
                        columnOrder: number) => {
                        const timeValues: ITimeValues = this.timeEntryRows[dataIndex][column.index];

                        return $("<div class='grid-cell'/>")
                            .width(column.width)
                            .text(this.convertNumberToTime(timeValues.timeActual).hours + "h "
                                + this.convertNumberToTime(timeValues.timeActual).minutes + "m");
                    },
                    index: "timeValues",
                    text: "Actual Time",
                    width: 100,
                },
                {
                    canSortBy: false,
                    getCellContents: (
                        rowInfo: IGridRowInfo,
                        dataIndex: number,
                        expandedState: number,
                        level: number,
                        column: IGridColumn,
                        indentIndex: number,
                        columnOrder: number) => {
                        const timeValues: ITimeValues = this.timeEntryRows[dataIndex][column.index];

                        return $("<div class='grid-cell'/>")
                            .width(column.width)
                            .text(this.convertNumberToTime(timeValues.timeEstimated).hours + "h "
                                + this.convertNumberToTime(timeValues.timeEstimated).minutes + "m");
                    },
                    index: "timeValues",
                    text: "Estimated Time",
                    width: 100,
                },
            ],
            height: "100%",
            source: this.timeEntryRows,
            width: "100%",
        };

        if (!this.gridControl) {
            this.gridControl = Controls.create(Grids.Grid, container, gridOptions);
        } else {
            this.gridControl.setDataSource(this.timeEntryRows);
        }

    }

    private getTimeEntries(): void {
        this.timeEntryService.getTimeEntries()
            .then((rows: ITimeEntryRow[]) => {
                this.timeEntryRows = rows;
                this.initGrid();
            }, () => {
                Notification.showGlobalError("Error loading Time entries.", "grid");
            });
    }
}

VSS.register(VSS.getContribution().id, () => {
    return new TimeEntryForm();
});

VSS.notifyLoadSucceeded();
