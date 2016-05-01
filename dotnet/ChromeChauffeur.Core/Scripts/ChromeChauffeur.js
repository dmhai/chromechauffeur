(function () {
    var cc = {};

    cc.write = function(text, selector) {
        var element = document.querySelector(selector);
        element.value = text;
        element.dispatchEvent(new Event("input"));
    };

    cc.click = function(selector) {
        document.querySelector(selector).click();
    };

    cc.exists = function(selector) {
        return document.querySelector(selector) !== null;
    };

    cc.selectByValue = function(selector, value) {
        cc.selectByPredicate(selector, function (opt) {
            return opt.value === value;
        });
    };

    cc.selectByText = function (selector, text) {
        cc.selectByPredicate(selector, function(opt) {
            return opt.text === text;
        });
    };

    cc.selectByPredicate = function(selector, predicate) {
        var selectElement = document.querySelector(selector);
        var options = selectElement.options;

        for (var i = 0; i < options.length; i++) {
            var isMatch = predicate(options[i]);

            if (isMatch) {
                selectElement.selectedIndex = i;
                break;
            }
        }
    };

    cc.selectByIndex = function(selector, index) {
        var selectElement = document.querySelector(selector);
        selectElement.selectedIndex = index;
    };

    cc.bypassConfirmationPopup = function(autoResponse) {
        window.confirm = function() {
            return autoResponse;
        };
    }

    cc.bypassAlertPopup = function () {
        window.alert = function () { };
    }

    window.chromeChauffeur_privates = cc;
})();
