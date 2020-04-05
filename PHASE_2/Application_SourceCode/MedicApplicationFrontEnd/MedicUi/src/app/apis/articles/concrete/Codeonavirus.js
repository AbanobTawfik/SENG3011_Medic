"use strict";
// WHO
// https://www.codeonavirus.com/
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
var Codeonavirus = /** @class */ (function (_super) {
    __extends(Codeonavirus, _super);
    function Codeonavirus() {
        var _this = _super.call(this, "https://www.codeonavirus.com", "/who/articles") || this;
        _this.name = 'Codeonavirus';
        return _this;
    }
    ////////////////////////////////////////////////////////////////////
    // Building a Request
    Codeonavirus.prototype.makeMethod = function () {
        return 'PUT';
    };
    Codeonavirus.prototype.makeQueryString = function (startDate, endDate, keyTerms, location, pageNumber) {
        return '';
    };
    Codeonavirus.prototype.makeBody = function (startDate, endDate, keyTerms, location, pageNumber) {
        var body = {
            start_date: this.startDateValue(startDate),
            end_date: this.endDateValue(endDate)
        };
        if (keyTerms.length > 0) {
            body['key_terms'] = this.keyTermsValue(keyTerms);
        }
        if (location) {
            body['location'] = this.locationValue(location);
        }
        return {
            body: JSON.stringify(body)
        };
    };
    Codeonavirus.prototype.makeHeaders = function () {
        return {
            'Content-Type': 'application/json'
        };
    };
    ////////////////////////////////////////////////////////////////////
    // Parameter Values
    Codeonavirus.prototype.startDateValue = function (date) {
        return date.format('YYYY-MM-DD');
    };
    Codeonavirus.prototype.endDateValue = function (date) {
        return date.format('YYYY-MM-DD');
    };
    Codeonavirus.prototype.locationValue = function (location) {
        return location + ','.repeat(3);
    };
    ////////////////////////////////////////////////////////////////////
    // Post-Processing
    Codeonavirus.prototype.processResponse = function (responseJson) {
        return this.getArticlesFromResponse(responseJson);
    };
    Codeonavirus.prototype.getArticlesFromResponse = function (json) {
        var _this = this;
        // console.log(util.inspect(json, false, null, true));
        return json.map(function (resArticle) { return _this.toStandardArticle(resArticle); });
    };
    Codeonavirus.prototype.toStandardArticle = function (resArticle) {
        var _this = this;
        var url = resArticle['url'];
        var dateOfPublication = resArticle['date_of_publication'];
        var headline = resArticle['headline'];
        var mainText = resArticle['main_text'];
        var reports = resArticle['reports'].map(function (resReport) { return _this.toStandardReport(resReport); });
        return new StandardArticle_1["default"](url, dateOfPublication, headline, mainText, reports);
    };
    Codeonavirus.prototype.toStandardReport = function (resReport) {
        var _this = this;
        var diseases = resReport['diseases'];
        var syndromes = resReport['syndromes'];
        var event_date = resReport['event_date'];
        var locations = resReport['locations'].map(function (resLocation) { return _this.toStandardLocation(resLocation); });
        return new StandardReport_1["default"](diseases, syndromes, event_date, locations);
    };
    Codeonavirus.prototype.toStandardLocation = function (resLocation) {
        var country = resLocation['country'];
        var state = resLocation['state'];
        var city = resLocation['city'];
        var location = '';
        if (city) {
            location += ", " + city;
        }
        if (state) {
            location += ", " + state;
        }
        location = location.slice(2);
        return new StandardLocation_1["default"](country, location);
    };
    return Codeonavirus;
}(ArticleApi_1["default"]));
exports["default"] = Codeonavirus;
