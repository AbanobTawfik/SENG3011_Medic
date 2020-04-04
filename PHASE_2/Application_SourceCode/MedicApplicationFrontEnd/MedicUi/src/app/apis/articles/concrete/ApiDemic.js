"use strict";
// H5N1
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
var APIdemic = /** @class */ (function (_super) {
  __extends(APIdemic, _super);
  function APIdemic() {
    return (
      _super.call(this, "http://api-demic.herokuapp.com", "/v1.1/articles") ||
      this
    );
  }
  ////////////////////////////////////////////////////////////////////
  // Building a Request
  APIdemic.prototype.makeQueryString = function (
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
      query += "&key_term=" + this.keyTermsValue(keyTerms);
    }
    if (location) {
      query += "&location=" + this.locationValue(location);
    }
    query += "&limit=10";
    return query;
  };
  ////////////////////////////////////////////////////////////////////
  // Post-Processing
  APIdemic.prototype.processResponse = function (responseJson) {
    return this.getArticlesFromResponse(responseJson);
  };
  APIdemic.prototype.getArticlesFromResponse = function (json) {
    var _this = this;
    // console.log(util.inspect(json, false, null, true));
    return json["articles"].map(function (resArticle) {
      return _this.toStandardArticle(resArticle);
    });
  };
  APIdemic.prototype.toStandardArticle = function (resArticle) {
    var _this = this;
    var url = resArticle["url"];
    var dateOfPublication = resArticle["date_of_publication"];
    var headline = resArticle["headline"];
    var mainText = resArticle["main_text"];
    var reports = resArticle["reports"].map(function (resReport) {
      return _this.toStandardReport(resReport);
    });
    return new StandardArticle_1["default"](
      url,
      dateOfPublication,
      headline,
      mainText,
      reports
    );
  };
  APIdemic.prototype.toStandardReport = function (resReport) {
    var _this = this;
    var diseases = [];
    if (typeof resReport["disease"] === "string") {
      if (resReport["disease"] !== "") {
        diseases.push(resReport["disease"]);
      }
    } else if (typeof resReport["diseases"] === "object") {
      diseases.push.apply(diseases, resReport["diseases"]);
    }
    var syndromes = [];
    if (typeof resReport["syndrome"] === "string") {
      if (resReport["syndrome"] !== "") {
        syndromes.push(resReport["syndrome"]);
      }
    } else if (typeof resReport["syndromes"] === "object") {
      syndromes.push.apply(syndromes, resReport["syndromes"]);
    }
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
  APIdemic.prototype.toStandardLocation = function (resLocation) {
    return new StandardLocation_1["default"](
      resLocation["country"],
      resLocation["location"]
    );
  };
  return APIdemic;
})(ArticleApi_1["default"]);
exports["default"] = APIdemic;
