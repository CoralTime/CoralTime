import Q = require("q");
import { IPromise } from "q";
import Services = require("TFS/WorkItemTracking/Services");
import { IProjectContext } from "../models/iprojectContext";
import { ISettings, IUserSettings } from "../models/isettings";
import { ISystemOptions } from "../models/isystemOptions";

export class ConfigurationService {
    accessTokenPromise: PromiseLike<void>;

    private accessToken: string;
    private extensionSettings: ISettings;
    private systemOptions: ISystemOptions;
    private userSettings: IUserSettings;

    constructor() {
        this.load();
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

    getSystemOptions(): ISystemOptions {
        return this.systemOptions;
    }

    getUserSettings(): IUserSettings {
        return this.userSettings;
    }

    private load(): void {
        this.extensionSettings = {
            siteUrl: "https://" + "coralteamdev.coraltime.io",
        };
        // this.getKeyValueFromStorage("settings")
        //     .then((savedSettings) => {
        //         this.extensionSettings = (JSON.parse(savedSettings).data);
        //         this.$siteUrl.val(this.extensionSettings.siteUrl);
        //     });
        this.loadAccessToken().then(() => {
            this.setHeaders();
            this.loadSetupOptions();
        });
        this.loadSystemOptions();
    }

    setKeyValueInStorage(key, value): void {
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

    private loadAccessToken(): PromiseLike<any> {
        return this.accessTokenPromise = VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
        });
    }

    loadSetupOptions(): JQuery.jqXHR {
        const data = {
            VstsProjectId: this.getExtensionContext().projectId,
            VstsUserId: this.getExtensionContext().userId,
        };

        return $.post(this.extensionSettings.siteUrl + "/api/v1/VSTS/Setup", JSON.stringify(data))
            .done((res: IUserSettings) => {
                this.userSettings = res;
            })
            .fail(() => {
                console.log("Server error.");
                $(".ct-coraltime").append("<div class='ct-error'>Server error.</div>");
            });
    }

    private loadSystemOptions(): PromiseLike<void> {
        return Services.WorkItemFormService.getService().then((service) => {
            service.getFieldValues(["System.Id", "System.Title", "System.WorkItemType"]).then((value: any) => {
                this.systemOptions = value;
            });
        });
    }

    private setHeaders(): void {
        $.ajaxSetup({
            headers: {
                "Content-Type": "application/json",
                "VSTSToken": this.getAccessToken(),
            },
        });
    }
}
