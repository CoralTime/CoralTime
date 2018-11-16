export class Notification {
    static showGlobalError(error: string, target: "form" | "grid" = "form"): void {
        $(".ct-" + target).before($("<div/>", {
            class: "ct-message ct-error",
            text: error,
        }));
    }

    static showNotification(message: string, type: "success" | "error", target: "form" | "grid" = "form"): void {
        $(".ct-" + target).after($("<div/>", {
            class: "ct-message ct-" + type,
            text: message,
        }));

        setTimeout(() => {
            $(".ct-" + target + " + .ct-message").remove();
        }, 5000);
    }
}
