"use strict";
// CDC
// https://seng3011medics.azurewebsites.net/swagger/index.html
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
var Medics = /** @class */ (function (_super) {
    __extends(Medics, _super);
    function Medics() {
        var _this = _super.call(this, "https://seng3011medics.azurewebsites.net", "/api/Reports/GetArticles") || this;
        _this.name = 'Medics';
        _this.limit = 50;
        return _this;
    }
    ////////////////////////////////////////////////////////////////////
    // Building a Request
    Medics.prototype.makeQueryString = function (startDate, endDate, keyTerms, location, pageNumber) {
        var query = "?start_date=" + this.startDateValue(startDate) +
            ("&end_date=" + this.endDateValue(endDate));
        if (keyTerms && keyTerms.length > 0) {
            query += "&key_terms=" + this.keyTermsValue(keyTerms);
        }
        if (location) {
            query += "&location=" + this.locationValue(location);
        }
        query += "&max=" + this.limit;
        query += "&offset=" + this.limit * pageNumber;
        return query;
    };
    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    Medics.prototype.processResponse = function (responseJson) {
        return this.getArticlesFromResponse(responseJson);
    };
    Medics.prototype.getArticlesFromResponse = function (json) {
        var _this = this;
        // console.log(util.inspect(json, false, null, true));
        return json['articles'].map(function (resArticle) { return _this.toStandardArticle(resArticle); });
    };
    Medics.prototype.toStandardArticle = function (resArticle) {
        var _this = this;
        var url = resArticle['url'];
        var dateOfPublication = resArticle['date_of_publication'];
        var headline = resArticle['headline'];
        var mainText = resArticle['main_text'];
        var reports = resArticle['reports'].map(function (resReport) { return _this.toStandardReport(resReport); });
        return new StandardArticle_1["default"](url, dateOfPublication, headline, mainText, reports);
    };
    Medics.prototype.toStandardReport = function (resReport) {
        var _this = this;
        var diseases = resReport['diseases'];
        var syndromes = resReport['syndromes'];
        var event_date = resReport['event_date'];
        var locations = resReport['locations'].map(function (resLocation) { return _this.toStandardLocation(resLocation); });
        return new StandardReport_1["default"](diseases, syndromes, event_date, locations);
    };
    Medics.prototype.toStandardLocation = function (resLocation) {
        var country = resLocation['country'];
        var location = resLocation['location'];
        var geonamesId = resLocation['geonames_id'];
        return new StandardLocation_1["default"](country, location, geonamesId);
    };
    return Medics;
}(ArticleApi_1["default"]));
exports["default"] = Medics;
