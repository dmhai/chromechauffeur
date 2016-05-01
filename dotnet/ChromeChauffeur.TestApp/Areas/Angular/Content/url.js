cc.url = {};

(function() {

    function stripLeadingSlash(s) {
        var hasLeadingSlash = s[0] == '/';

        if (hasLeadingSlash) {
            s = s.substr(1);
        }

        return s;
    }

    cc.url.createContent = function (url) {
        return cc.url.create("/areas/angular/" + stripLeadingSlash(url));
    };

    cc.url.create = function (url) {

        var startsWithBaseUrlRegex = new RegExp("^" + cc.baseUrl, "i");

        if (startsWithBaseUrlRegex.test(url)) {
            return url;
        }

        return cc.baseUrl + stripLeadingSlash(url);
    };

})();

