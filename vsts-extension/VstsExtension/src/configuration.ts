// tslint:disable-next-line
/// <reference path="isettings.d.ts" />
// tslint:disable-next-line
/// <reference path="iprojectContext.ts"/>

import { IPromise } from "q";
import Q = require("q");
import Combos = require("VSS/Controls/Combos");
import WebApi_Constants = require("VSS/WebApi/Constants");
import "../static/_scss/styles.scss";
import { IProjectContext } from "./iprojectContext";

// import { settings } from "./telemetryClientSettings";

// import TelemetryClient = require("scripts/TelemetryClient");

export class Configuration {

    private extensionSettings: ISettings;
    private $siteUrl = $("#siteUrl-input");
    private $userName = $("#userName-input");
    private accessToken: string;
    constructor() {
        this.load();
    }

    public load() {
        this.getKeyValueFromStorage("settings")
            .then((savedSettings) => {
                this.extensionSettings = (JSON.parse(savedSettings).data);
                this.$siteUrl.val(this.extensionSettings.siteUrl);
                this.$userName.val(this.extensionSettings.userName);
            });
        this.loadAccessToken();
    }

    public onSave() {
        const settings = this.getCustomSettings();
        this.setKeyValueInStorage("settings", settings);
    }

    public getExtensionContext(): IProjectContext {
        const webContext = VSS.getWebContext();
        const extensionContext: IProjectContext = {
            projectId: webContext.project.id,
            projectName: webContext.project.name,
            userEmail: webContext.user.email,
            userName: webContext.user.uniqueName,
        };
        return extensionContext;
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

        const result = {
            data: JSON.stringify({
                siteUrl,
                userName,
            } as ISettings),
        };
        return result;
    }

    private setKeyValueInStorage(key, value): void {
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Set value in user scope
            dataService.setValue(key, value, { scopeType: "User" }).then((resvalue) => {
                alert(resvalue);
            });
        });
    }

    private getKeyValueFromStorage(key): IPromise<string> {
        const deferred = Q.defer<string>();
        VSS.getService(VSS.ServiceIds.ExtensionData).then((dataService: IExtensionDataService) => {
            // Get value from user scope
            dataService.getValue(key, { scopeType: "User" }).then((value) => {
                deferred.resolve(value as string);
            });
        });
        return deferred.promise;
    }

    private loadAccessToken() {
        VSS.getAppToken().then((token) => {
            this.accessToken = token.token;
        });
    }
}
