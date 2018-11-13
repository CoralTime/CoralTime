import Q = require("q");
import { IPromise } from "q";
import services = require("TFS/WorkItemTracking/Services");
import { IProjectContext, ISettings, IUserSettings, IWorkItemOptions } from "../models/settings";
import { Notification } from "../utils/notification";

export class ConfigurationService {
    accessTokenPromise: PromiseLike<void>;
    workItemFormPromise: PromiseLike<void>;

    private accessToken: string;
    private extensionSettings: ISettings;
    private userSettings: IUserSettings;
    private workItemOptions: IWorkItemOptions;

    constructor() {
        this.setExtensionSettings();
        this.loadWorkItemOptions();
        this.loadAccessToken();
        this.getSetupOptions().then((userSettings: IUserSettings) => {
            this.userSettings = userSettings;
        });
    }

    getAccessToken(): string {
        return this.accessToken;
    }

    getExtensionContext(): IProjectContext {
        const webContext = VSS.getWebContext();
        return {
            projectId: webContext.project.id,
            userId: webContext.user.id,
        };
    }

    getExtensionSettings(): ISettings {
        return this.extensionSettings;
    }

    getUserSettings(): IUserSettings {
        return this.userSettings;
    }

    getWorkItemOptions(): IWorkItemOptions {
        return this.workItemOptions;
    }

    private getSetupOptions(): IPromise<IUserSettings> {
        const deferred = Q.defer<IUserSettings>();
        this.getKeyValueFromStorage("userSettings").then((res: IUserSettings) => {
            if (res) {
                deferred.resolve(res);
            } else {
                this.loadSetupOptions().then((res) => {
                    deferred.resolve(res);
                });
            }
        });

        return deferred.promise;
    }

    private getKeyValueFromStorage(key: string): IPromise<any> {
        const deferred = Q.defer<any>();
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Get value from user scope
            dataService.getValue(key, {scopeType: "User"}).then((value) => {
                deferred.resolve(value);
            });
        });
        return deferred.promise;
    }

    private loadAccessToken(): void {
        this.accessTokenPromise = VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
            this.setHeaders();
        });
    }

    private loadSetupOptions(): IPromise<IUserSettings> {
        const deferred = Q.defer<IUserSettings>();
        const data = {
            VstsProjectId: this.getExtensionContext().projectId,
            VstsUserId: this.getExtensionContext().userId,
        };

        this.accessTokenPromise.then(() => {
            $.post(this.extensionSettings.siteUrl + "/api/v1/VSTS/Setup", JSON.stringify(data))
                .done((res: IUserSettings) => {
                    const userSettings = {
                        memberId: res.memberId,
                        projectId: res.projectId,
                    };
                    this.setKeyValueInStorage("userSettings", userSettings);
                    deferred.resolve(userSettings);
                })
                .fail(() => {
                    Notification.showGlobalError("Error loading project settings.");
                    deferred.reject(null);
                });
        });

        return deferred.promise;
    }

    private loadWorkItemOptions(): void {
        this.workItemFormPromise = services.WorkItemFormService.getService().then((service) => {
            return service.getFieldValues(["System.Id", "System.Title", "System.WorkItemType"]).then((value: any) => {
                return this.workItemOptions = value;
            });
        });
    }

    private setExtensionSettings(): void {
        const siteUrl = VSS.getConfiguration().witInputs["SiteUrl"];

        if (!/^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$/.test(siteUrl)) {
            Notification.showGlobalError(siteUrl + " has invalid format.");
            return;
        }

        this.extensionSettings = {
            siteUrl: "https://" + siteUrl,
        };
    }

    private setHeaders(): void {
        // Set headers for all jQuery requests
        $.ajaxSetup({
            headers: {
                "Content-Type": "application/json",
                "VSTSToken": this.getAccessToken(),
            },
        });
    }

    private setKeyValueInStorage(key: string, value: any): void {
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Set value in user scope
            dataService.setValue(key, value, {scopeType: "User"});
        });
    }
}
