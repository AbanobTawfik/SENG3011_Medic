"use strict";
// WHO
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
var AUTH_TOKEN = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImFiYzEyM0BnbWFpbC5jb20iLCJ0aW1lUmVnaXN0ZXJlZCI6IjAyLzA0LzIwMjAsIDE2OjQ1OjU2In0.-CH5qcrdDxqPSs94B-NrR3b8Q4B83maBYOUL9V0egSc';
var Webbscrapers = /** @class */ (function (_super) {
    __extends(Webbscrapers, _super);
    function Webbscrapers() {
        return _super.call(this, "https://webbscrapers.live", "/reports") || this;
    }
    ////////////////////////////////////////////////////////////////////
    // Building a Request
    Webbscrapers.prototype.makeQueryString = function (startDate, endDate, keyTerms, location) {
        var query = "?startDateTime=" + this.startDateValue(startDate) +
            ("&endDateTime=" + this.endDateValue(endDate));
        if (keyTerms && keyTerms.length > 0) {
            query += "&keyTerms=" + this.keyTermsValue(keyTerms);
        }
        if (location) {
            query += "&location=" + this.locationValue(location);
        }
        return query;
    };
    Webbscrapers.prototype.makeHeaders = function () {
        return {
            'Authorization': "Bearer " + AUTH_TOKEN
        };
    };
    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    Webbscrapers.prototype.processResponse = function (responseJson) {
        return this.getArticlesFromResponse(responseJson);
    };
    Webbscrapers.prototype.getArticlesFromResponse = function (json) {
        var _this = this;
        if (typeof (json.result[0].Error) !== 'undefined') {
            return [];
        }
        else {
            // console.log(util.inspect(json, false, null, true));
            return json['result'].map(function (resArticle) { return _this.toStandardArticle(resArticle); });
        }
    };
    Webbscrapers.prototype.toStandardArticle = function (resArticle) {
        var _this = this;
        var url = resArticle['url'];
        var dateOfPublication = resArticle['date_of_publication'];
        var headline = resArticle['headline'];
        var mainText = resArticle['main_text'];
        var reports = resArticle['reports'].map(function (resReport) { return _this.toStandardReport(resReport); });
        return new StandardArticle_1["default"](url, dateOfPublication, headline, mainText, reports);
    };
    Webbscrapers.prototype.toStandardReport = function (resReport) {
        var _this = this;
        var diseases = resReport['diseases'];
        var syndromes = resReport['syndromes'];
        var event_date = resReport['event_date'];
        var locations = resReport['locations'].map(function (resLocation) { return _this.toStandardLocation(resLocation); });
        return new StandardReport_1["default"](diseases, syndromes, event_date, locations);
    };
    Webbscrapers.prototype.toStandardLocation = function (resLocation) {
        var geonamesId = resLocation['geonames-id'];
        return new StandardLocation_1["default"](null, null, geonamesId);
    };
    return Webbscrapers;
}(ArticleApi_1["default"]));
exports["default"] = Webbscrapers;
