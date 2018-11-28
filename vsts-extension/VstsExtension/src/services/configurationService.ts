import Q = require("q");
import { IPromise } from "q";
import Services = require("TFS/WorkItemTracking/Services");
import { IProjectContext, ISettings, IUserSettings, IWorkItemOptions } from "../models/settings";
import { Notification } from "../utils/notification";

export class ConfigurationService {
    accessTokenPromise: PromiseLike<void>;
    extensionSettingsPromise: IPromise<ISettings>;
    workItemFormPromise: PromiseLike<void>;

    protected projectContext: IProjectContext = this.getExtensionContext();
    protected userSettings: IUserSettings;
    private accessToken: string;
    private extensionSettings: ISettings;
    private workItemOptions: IWorkItemOptions;

    constructor() {
        this.loadWorkItemOptions();
        this.loadAccessToken();
        this.loadUserSettings().then((userSettings: IUserSettings) => {
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

    getWorkItemOptions(): IWorkItemOptions {
        return this.workItemOptions;
    }

    getKeyValueFromStorage(key: string, scopeType: "User" | null): IPromise<any> {
        const deferred = Q.defer<any>();
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Get value from scope
            dataService.getValue(key, scopeType ? {scopeType: "User"} : null).then((value) => {
                deferred.resolve(value);
            });
        });
        return deferred.promise;
    }

    setKeyValueInStorage(key: string, value: any, scopeType: "User" | null): PromiseLike<void> {
        return VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Set value in scope
            dataService.setValue(key, value, scopeType ? {scopeType: "User"} : null);
        });
    }

    private loadAccessToken(): void {
        this.accessTokenPromise = VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
            this.setHeaders();
        });
    }

    private loadWorkItemOptions(): void {
        this.workItemFormPromise = Services.WorkItemFormService.getService().then((service) => {
            return service.getFieldValues(["System.Id", "System.Title", "System.WorkItemType"]).then((value: any) => {
                return this.workItemOptions = value;
            });
        });
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

    // EXTENSION SETTINGS

    getUserSettings(): IUserSettings {
        return this.userSettings;
    }

    loadUserSettings(): IPromise<IUserSettings> {
        const deferred = Q.defer<IUserSettings>();
        this.getKeyValueFromStorage("userSettings", "User").then((res: IUserSettings) => {
            if (res && res.expirationDate > new Date().getTime()) {
                deferred.resolve(res);
            } else {
                this.updateUserSettings().then((res: IUserSettings) => {
                    deferred.resolve(res);
                });
            }
        });

        return deferred.promise;
    }

    updateUserSettings(): IPromise<IUserSettings> {
        const deferred = Q.defer<IUserSettings>();
        const data = {
            VstsProjectId: this.getExtensionContext().projectId,
            VstsUserId: this.getExtensionContext().userId,
        };

        Promise.all([this.accessTokenPromise, this.extensionSettingsPromise]).then(() => {
            $.post(this.getExtensionSettings().siteUrl + "/api/v1/VSTS/Setup", JSON.stringify(data))
                .done((res: IUserSettings) => {
                    const userSettings = {
                        expirationDate: new Date().getTime() + 24 * 3600 * 1000,
                        memberId: res.memberId,
                        projectId: res.projectId,
                    };
                    this.setKeyValueInStorage("userSettings", userSettings, "User");
                    deferred.resolve(userSettings);
                })
                .fail(() => {
                    Notification.showGlobalError("Error loading project settings.", "form");
                    deferred.reject(null);
                });
        });

        return deferred.promise;
    }

    // USER SETTINGS

    loadExtensionSettings(): IPromise<ISettings> {
        const deferred = Q.defer<ISettings>();
        this.getKeyValueFromStorage("extensionSettings", null).then((res: ISettings) => {
            this.extensionSettings = {
                siteUrl: "https://" + res.siteUrl,
            };

            deferred.resolve(this.extensionSettings);
        });

        return this.extensionSettingsPromise = deferred.promise;
    }

    setExtensionSettings(siteUrl: string): IPromise<any> {
        const deferred = Q.defer();
        this.setKeyValueInStorage("extensionSettings", {siteUrl}, null).then(() => {
            this.extensionSettings = {
                siteUrl: "https://" + siteUrl,
            };

            deferred.resolve();
        });

        return deferred.promise;
    }
}
