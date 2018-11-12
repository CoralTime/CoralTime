export class Notification {
    static showGlobalError(error: string): void {
        $(".ct-coraltime form").before($("<div/>", {
            class: "ct-message ct-error",
            text: error,
        }));
    }

    static showNotification(message: string, type: "success" | "error"): void {
        $(".ct-coraltime form").after($("<div/>", {
            class: "ct-message ct-" + type,
            text: message,
        }));

        setTimeout(() => {
            $(".ct-coraltime > .ct-message").remove();
        }, 5000);
    }
}
