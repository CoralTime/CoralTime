export class Notification {
    static showGlobalError(error: string, target: "form" | "grid"): void {
        $(".ct-" + target).before($("<div/>", {
            class: "ct-message ct-error",
            text: error,
        }));
    }

    static removeGlobalErrors(): void {
        $(".ct-message.ct-error").remove();
    }

    static showNotification(message: string, type: "success" | "error", target: "form" | "grid" | "configuration"): void {
        $(".ct-" + target).after($("<div/>", {
            class: "ct-message ct-" + type,
            text: message,
        }));

        setTimeout(() => {
            $(".ct-" + target + " + .ct-message").remove();
        }, 3000);
    }
}
