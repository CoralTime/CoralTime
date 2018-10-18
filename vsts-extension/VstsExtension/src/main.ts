import { Configuration } from "./configuration";
import { TimeEntryService } from "./timeEntryService";

const context = VSS.getExtensionContext();
VSS.register(context.publisherId + "." + context.extensionId + "." + "CoralTimeTracker", () => {
    return new TimeEntryService();
});

VSS.register(context.publisherId + "." + context.extensionId + "." + "CoralTimeTracker-Configuration", () => {
    return new Configuration();
});
