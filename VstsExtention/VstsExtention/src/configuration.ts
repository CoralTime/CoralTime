// tslint:disable-next-line
/// <reference path="isettings.d.ts" />
// tslint:disable-next-line
/// <reference path="iextentionContext.ts"/>

import Q = require("q");
import Combos = require("VSS/Controls/Combos");
import WebApi_Constants = require("VSS/WebApi/Constants");
import { IExtensionContext } from "./iextentionContext";

// import TelemetryClient = require("scripts/TelemetryClient");

export class Configuration {
    private static $dateTimeCombo: Combos.Combo = null;

    private widgetConfigurationContext = null;
    private extentionSettings = null;
    private $siteUrl = $("#siteUrl-input");
    private $userName = $("#userName-input");
    private $password = $("#password-input");
    private $isSSO = $("#isSSO-input");

    private currentIterationEnd = null;
    constructor(public WidgetHelpers, public isSprintWidget: boolean) { }

    public load(widgetSettings, widgetConfigurationContext) {
        this.widgetConfigurationContext = widgetConfigurationContext;
        const settings: ISettings = JSON.parse(widgetSettings.customSettings.data);
        this.extentionSettings = this.getExtensionContext();

        return this.WidgetHelpers.WidgetStatusHelper.Success();
    }

    public onSave() {
        const isValid = true;
        if (isValid) {

            // TelemetryClient.TelemetryClient.getClient().trackEvent("Updated configuration");
            return this.WidgetHelpers.WidgetConfigurationSave.Valid(this.getCustomSettings());
        } else {
            return this.WidgetHelpers.WidgetConfigurationSave.Invalid();
        }
    }

    private getCustomSettings() {
        const siteUrl = (this.$siteUrl.val() as string);
        const userName = (this.$userName.val() as string);
        const password = (this.$password.val() as string);
        const isSSO = (this.$isSSO.prop("checked") as boolean);

        const result = {
            data: JSON.stringify({
                isSSO,
                password,
                siteUrl,
                userName,
            } as ISettings),
        };
        return result;
    }

    private getExtensionContext(): IExtensionContext {
        const webContext = VSS.getWebContext();
        const extensionContext: IExtensionContext = {
            projectId: webContext.project.id,
            projectName: webContext.project.name,
            userEmail: webContext.user.email,
            userName: webContext.user.uniqueName,
        };

        return extensionContext;
    }
}
