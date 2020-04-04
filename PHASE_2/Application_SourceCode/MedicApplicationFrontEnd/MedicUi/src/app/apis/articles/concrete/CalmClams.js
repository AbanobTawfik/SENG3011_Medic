"use strict";
// Global Incident Map
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
exports.__esModule = true;
var StandardArticle_1 = require("../../../types/StandardArticle");
var StandardLocation_1 = require("../../../types/StandardLocation");
var StandardReport_1 = require("../../../types/StandardReport");
var ArticleApi_1 = require("../base/ArticleApi");
var CalmClams = /** @class */ (function (_super) {
    __extends(CalmClams, _super);
    function CalmClams() {
        return _super.call(this, "http://calmclams.appspot.com", "/disease_reports") || this;
    }
    ////////////////////////////////////////////////////////////////////
    // Building a Request
    CalmClams.prototype.makeQueryString = function (startDate, endDate, keyTerms, location) {
        var query = "?start_date=" + this.startDateValue(startDate) +
            ("&end_date=" + this.endDateValue(endDate));
        if (keyTerms && keyTerms.length > 0) {
            query += "&keyterms=" + this.keyTermsValue(keyTerms);
        }
        if (location) {
            query += "&location=" + this.locationValue(location);
        }
        return query;
    };
    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    CalmClams.prototype.processResponse = function (responseJson) {
        return this.getArticlesFromResponse(responseJson);
    };
    CalmClams.prototype.getArticlesFromResponse = function (json) {
        var _this = this;
        // console.log(util.inspect(json, false, null, true));
        return json['articles'].map(function (resArticle) { return _this.toStandardArticle(resArticle); });
    };
    CalmClams.prototype.toStandardArticle = function (resArticle) {
        var _this = this;
        var url = resArticle['url'];
        var dateOfPublication = resArticle['date_of_publication'];
        var headline = resArticle['headline'];
        var mainText = resArticle['main_text'];
        var reports = resArticle['reports'].map(function (resReport) { return _this.toStandardReport(resReport); });
        return new StandardArticle_1["default"](url, dateOfPublication, headline, mainText, reports);
    };
    CalmClams.prototype.toStandardReport = function (resReport) {
        var _this = this;
        var diseases = resReport['diseases'];
        var syndromes = resReport['syndromes'];
        var event_date = resReport['event_date'];
        var locations = resReport['locations'].map(function (resLocation) { return _this.toStandardLocation(resLocation); });
        return new StandardReport_1["default"](diseases, syndromes, event_date, locations);
    };
    CalmClams.prototype.toStandardLocation = function (resLocation) {
        var country = resLocation['country'];
        var location = resLocation['location'];
        var coordinates = this.toCoordinates(resLocation['coords']);
        return new StandardLocation_1["default"](country, location, null, null, coordinates);
    };
    CalmClams.prototype.toCoordinates = function (coordinatesString) {
        if (!coordinatesString)
            return null;
        if (coordinatesString.search(/, ?/) === -1)
            return null;
        var coordinates = coordinatesString.split(/, ?/);
        return {
            latitude: parseFloat(coordinates[0]),
            longitude: parseFloat(coordinates[1])
        };
    };
    return CalmClams;
}(ArticleApi_1["default"]));
exports["default"] = CalmClams;
