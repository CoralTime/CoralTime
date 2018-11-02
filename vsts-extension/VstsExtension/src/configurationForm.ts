import { ConfigurationService } from "./services/configurationService";

export class ConfigurationForm {
    private configurationService: ConfigurationService;
    private isSiteUrlInputValid: boolean = false;
    private formCallbacks = [];

    private $siteUrlInput = $("#siteUrl-input");
    private $siteUrlErrorText = $(".ct-validation-error");

    constructor(private widgetHelpers) {
        this.configurationService = new ConfigurationService();
        this.load();
    }

    load(): void {
        this.addEventListeners();
        return this.widgetHelpers.WidgetStatusHelper.Success();
    }

    private getCustomSettings() {
        const siteUrl = (this.$siteUrlInput.val() as string);

        return {
            data: JSON.stringify({
                siteUrl,
            } as ISettings),
        };
    }

    // FORM
    attachFormChanged(cb) {
        this.formCallbacks.push(cb);
    }

    getFormData() {
        return {
            siteUrl: this.$siteUrlInput.val(),
        };
    }

    isFormValid(): boolean {
        return this.isSiteUrlInputValid;
    }

    onSave(): void {
        const settings = this.getCustomSettings();
        this.configurationService.setKeyValueInStorage("settings", settings);
    }

    private addEventListeners(): void {
        this.$siteUrlInput.on("change", this.inputChanged.bind(this));
        this.$siteUrlInput.on("input", this.validateInput.bind(this));
    }

    private inputChanged(): void {
        this.formCallbacks.forEach((value, i) => this.formCallbacks[i](this.isFormValid()));
    }

    private validateInput(): void {
        this.isSiteUrlInputValid = this.$siteUrlInput.val() !== "";

        if (!this.isSiteUrlInputValid) {
            this.$siteUrlErrorText.text("Please enter your name.");
        }

        this.$siteUrlErrorText.css("visibility", this.isSiteUrlInputValid ? "hidden" : "visible");
    }
}

const context = VSS.getExtensionContext();
const dialogContributionId = context.publisherId + "." + context.extensionId + ".ConfigurationDialog";

VSS.require(["TFS/Dashboards/WidgetHelpers"], (WidgetHelpers) => {
    VSS.register(context.publisherId + "." + context.extensionId + ".ConfigurationDialogButton", () => {
        return {
            // Called when the menu item is clicked.
            execute: () => {
                let form: any;
                VSS.getService(VSS.ServiceIds.Dialog).then((dialogService: IHostDialogService) => {
                    const dialogOptions = {
                        cancelText: "Cancel",
                        getDialogResult: () => {
                            return form ? form.getFormData() : null;
                        },
                        height: 200,
                        okCallback: (result) => {
                            console.log(JSON.stringify(result));
                        },
                        okText: "Save Settings",
                        resizable: false,
                        title: "CoralTimeTracker Configuration",
                        width: 400,
                    } as IHostDialogOptions;

                    dialogService.openDialog(dialogContributionId, dialogOptions).then((dialog) => {
                        // Get configurationForm instance which is registered in coraltimetrackerconfiguration.html
                        dialog.getContributionInstance(dialogContributionId).then((formInstance) => {

                            // Keep a reference of form instance (to be used above in dialog options)
                            form = formInstance;

                            // Subscribe to form input changes and update the Ok enabled state
                            form.attachFormChanged((isValid) => {
                                dialog.updateOkButton(isValid);
                            });
                            // Set the initial ok enabled state
                            form.isFormValid().then((isValid) => {
                                dialog.updateOkButton(isValid);
                            });
                        });
                    });
                });
            },
        };
    });

    VSS.notifyLoadSucceeded();
});

VSS.require(["TFS/Dashboards/WidgetHelpers"], (WidgetHelpers) => {
    VSS.register(dialogContributionId, () => {
        return new ConfigurationForm(WidgetHelpers);
    });

    VSS.notifyLoadSucceeded();
});
