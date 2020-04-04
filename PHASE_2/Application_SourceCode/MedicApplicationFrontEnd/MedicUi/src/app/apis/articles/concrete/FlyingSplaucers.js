"use strict";
// CDC
var __extends =
  (this && this.__extends) ||
  (function () {
    var extendStatics = function (d, b) {
      extendStatics =
        Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array &&
          function (d, b) {
            d.__proto__ = b;
          }) ||
        function (d, b) {
          for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
        };
      return extendStatics(d, b);
    };
    return function (d, b) {
      extendStatics(d, b);
      function __() {
        this.constructor = d;
      }
      d.prototype =
        b === null
          ? Object.create(b)
          : ((__.prototype = b.prototype), new __());
    };
  })();
exports.__esModule = true;
var StandardArticle_1 = require("../../../types/StandardArticle");
var StandardLocation_1 = require("../../../types/StandardLocation");
var StandardReport_1 = require("../../../types/StandardReport");
var ArticleApi_1 = require("../base/ArticleApi");
var FlyingSplaucers = /** @class */ (function (_super) {
  __extends(FlyingSplaucers, _super);
  function FlyingSplaucers() {
    return (
      _super.call(
        this,
        "https://us-central1-flyingsplaucers-7b3cf.cloudfunctions.net",
        "/reports/reports"
      ) || this
    );
  }
  ////////////////////////////////////////////////////////////////////
  // Building a Request
  FlyingSplaucers.prototype.makeQueryString = function (
    startDate,
    endDate,
    keyTerms,
    location
  ) {
    var query =
      "?start_date=" +
      this.startDateValue(startDate) +
      ("&end_date=" + this.endDateValue(endDate));
    if (keyTerms && keyTerms.length > 0) {
      query += "&key_terms=" + this.keyTermsValue(keyTerms);
    }
    if (location) {
      query += "&location=" + this.locationValue(location);
    }
    return query;
  };
  ////////////////////////////////////////////////////////////////////
  // Post-Processing
  FlyingSplaucers.prototype.processResponse = function (responseJson) {
    return this.getArticlesFromResponse(responseJson);
  };
  FlyingSplaucers.prototype.getArticlesFromResponse = function (json) {
    var _this = this;
    // console.log(util.inspect(json, false, null, true));
    return json.map(function (resArticle) {
      return _this.toStandardArticle(resArticle);
    });
  };
  FlyingSplaucers.prototype.toStandardArticle = function (resArticle) {
    var url = resArticle["url"];
    var dateOfPublication = resArticle["date_of_publication"];
    var headline = resArticle["headline"];
    var mainText = resArticle["main_text"];
    var reports = [this.toStandardReport(resArticle["reports"])];
    return new StandardArticle_1["default"](
      url,
      dateOfPublication,
      headline,
      mainText,
      reports
    );
  };
  FlyingSplaucers.prototype.toStandardReport = function (resReport) {
    var _this = this;
    var diseases = resReport["diseases"];
    var syndromes = resReport["syndromes"];
    var event_date = resReport["event_date"];
    var locations = resReport["locations"].map(function (resLocation) {
      return _this.toStandardLocation(resLocation);
    });
    return new StandardReport_1["default"](
      diseases,
      syndromes,
      event_date,
      locations
    );
  };
  FlyingSplaucers.prototype.toStandardLocation = function (resLocation) {
    var country = resLocation["country"];
    var location = resLocation["location"];
    return new StandardLocation_1["default"](country, location);
  };
  return FlyingSplaucers;
})(ArticleApi_1["default"]);
exports["default"] = FlyingSplaucers;
