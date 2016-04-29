(function () {
    var cc = {};

    cc.write = function(text, selector) {
        document.querySelector(selector).value = text;
    };

    cc.click = function(selector) {
        document.querySelector(selector).click();
    };

    cc.exists = function(selector) {
        return document.querySelector(selector) !== null;
    };

    window.chromeChauffeur_privates = cc;
})();
