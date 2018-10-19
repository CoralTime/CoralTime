import Q = require("q");
import { IPromise } from "q";
import { IProjectContext } from "./models/iprojectContext";

export class ConfigurationService {

    public accessTokenPromise: PromiseLike<void>;
    private accessToken: string;
    private extensionSettings: ISettings;
    private $siteUrl = $("#siteUrl-input");
    private $userName = $("#userName-input");

    constructor() {
        this.load();
    }

    public load(): void {
        this.extensionSettings = {
            siteUrl: "coralteam.coraltime.io",
            userName: "Roman",
        };
        // this.getKeyValueFromStorage("settings")
        //     .then((savedSettings) => {
        //         this.extensionSettings = (JSON.parse(savedSettings).data);
        //         this.$siteUrl.val(this.extensionSettings.siteUrl);
        //         this.$userName.val(this.extensionSettings.userName);
        //     });
        this.loadAccessToken();
    }

    public onSave(): void {
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
