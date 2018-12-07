export class Loading {
    static addLoading(selector: JQuery): void {
        const el = $(selector);
        el.parent().addClass("ct-loading");
        el.before($("<div/>", {
            class: "big-status-progress",
        }));
    }

    static removeLoading(selector: JQuery): void {
        const parent = $(selector).parent();
        parent.removeClass("ct-loading");
        parent.find(".big-status-progress").remove();
    }
}
