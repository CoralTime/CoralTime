import Q = require("q");
import { IPromise } from "q";
import { IProjectContext } from "../models/iprojectContext";

export class ConfigurationService {
    accessTokenPromise: PromiseLike<void>;

    private accessToken: string;
    private extensionSettings: ISettings;

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
            projectName: webContext.project.name,
            userEmail: webContext.user.email,
            userName: webContext.user.uniqueName,
        };
    }

    getSettings(): ISettings {
        return this.extensionSettings;
    }

    load(): void {
        const that = this;
        this.extensionSettings = {
            siteUrl: "https://" + "coralteamdev.coraltime.io",
        };
        // this.getKeyValueFromStorage("settings")
        //     .then((savedSettings) => {
        //         this.extensionSettings = (JSON.parse(savedSettings).data);
        //         this.$siteUrl.val(this.extensionSettings.siteUrl);
        //     });
        this.loadAccessToken();

        // return that.WidgetHelpers.WidgetStatusHelper.Success();
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

    private loadAccessToken(): void {
        this.accessTokenPromise = VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
        });
        // VSS.getAccessToken().then((token) => {
        // });
    }
}
