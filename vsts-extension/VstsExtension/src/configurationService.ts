import Q = require("q");
import { IPromise } from "q";
import Dialogs = require("VSS/Controls/Dialogs");
import { IProjectContext } from "./models/iprojectContext";

export class ConfigurationService {

    public accessTokenPromise: PromiseLike<void>;
    private accessToken: string;
    private extensionSettings: ISettings;
    private $siteUrl = $("#siteUrl-input");
    private $userName = $("#userName-input");

    constructor(public WidgetHelpers) {
        this.load();
    }

    public load(): void {
        const that = this;
        this.extensionSettings = {
            siteUrl: "https://coralteamdev.coraltime.io",
            userName: "Roman",
        };
        // this.getKeyValueFromStorage("settings")
        //     .then((savedSettings) => {
        //         this.extensionSettings = (JSON.parse(savedSettings).data);
        //         this.$siteUrl.val(this.extensionSettings.siteUrl);
        //         this.$userName.val(this.extensionSettings.userName);
        //     });
        this.loadAccessToken();

        return that.WidgetHelpers.WidgetStatusHelper.Success();
    }

    public onSave(): void {
        console.log(33);
        const settings = this.getCustomSettings();
        this.setKeyValueInStorage("settings", settings);
    }

    public getExtensionContext(): IProjectContext {
        const webContext = VSS.getWebContext();
        return {
            projectId: webContext.project.id,
            projectName: webContext.project.name,
            userEmail: webContext.user.email,
            userName: webContext.user.uniqueName,
        };
    }

    public getSettings(): ISettings {
        return this.extensionSettings;
    }

    public getAccessToken(): string {
        return this.accessToken;
    }

    private getCustomSettings() {
        const siteUrl = (this.$siteUrl.val() as string);
        const userName = (this.$userName.val() as string);

        return {
            data: JSON.stringify({
                siteUrl,
                userName,
            } as ISettings),
        };
    }

    private setKeyValueInStorage(key, value): void {
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Set value in user scope
            dataService.setValue(key, value, {scopeType: "User"}).then((resvalue) => {
                alert(resvalue);
            });
        });
    }

    private getKeyValueFromStorage(key): IPromise<string> {
        const deferred = Q.defer<string>();
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Get value from user scope
            dataService.getValue(key, {scopeType: "User"}).then((value) => {
                deferred.resolve(value as string);
            });
        });
        return deferred.promise;
    }

    private loadAccessToken(): void {
        this.accessTokenPromise = VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
        });
    }
}

const context = VSS.getExtensionContext();
const dialogContributionId = context.publisherId + "." + context.extensionId + ".ConfigurationDialog";

VSS.require(["TFS/Dashboards/WidgetHelpers"], (WidgetHelpers) => {
    VSS.register(context.publisherId + "." + context.extensionId + ".ConfigurationDialogButton", () => {
        return {
            // Called when the menu item is clicked.
            execute: (actionContext) => {
                let form;
                VSS.getService(VSS.ServiceIds.Dialog).then((dialogService: any) => {
                    const dialogOptions = {
                        cancelText: "Cancel",
                        getDialogResult: () => {
                            return form ? form.getFormData() : null;
                        },
                        height: 600,
                        okCallback: (result) => {
                            console.log(JSON.stringify(result));
                        },
                        okText: "Save Settings",
                        title: "CoralTimeTracker Configuration",
                        width: 800,
                    } as Dialogs.IModalDialogOptions;

                    dialogService.openDialog(dialogContributionId, dialogOptions).then((dialog) => {
                        dialog.getContributionInstance("ConfigurationDialog").then((formInstance) => {
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
        return new ConfigurationService(WidgetHelpers);
    });

    VSS.notifyLoadSucceeded();
});
