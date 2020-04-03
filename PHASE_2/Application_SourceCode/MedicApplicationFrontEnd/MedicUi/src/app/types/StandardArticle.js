"use strict";
// Standard article object
exports.__esModule = true;
var StandardArticle = /** @class */ (function () {
    function StandardArticle(url, dateOfPublication, headline, mainText, reports, id) {
        if (id === void 0) { id = null; }
        this.url = url;
        this.dateOfPublication = dateOfPublication;
        this.headline = headline;
        this.mainText = mainText;
        this.reports = reports;
        this.id = id;
    }
    return StandardArticle;
}());
exports["default"] = StandardArticle;
