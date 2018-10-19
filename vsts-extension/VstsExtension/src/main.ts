import { ConfigurationService } from "./configurationService";
import { TimeEntryService } from "./timeEntryService";

const context = VSS.getExtensionContext();
VSS.register(context.publisherId + "." + context.extensionId + "." + "CoralTimeTracker", () => {
    return new TimeEntryService(new ConfigurationService());
});

VSS.register(context.publisherId + "." + context.extensionId + "." + "CoralTimeTracker-Configuration", () => {
    return new ConfigurationService();
});
