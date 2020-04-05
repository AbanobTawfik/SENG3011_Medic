"use strict";
exports.__esModule = true;
var ArticleRequest_1 = require("../request/ArticleRequest");
var ApiDemic_1 = require("../concrete/ApiDemic");
var CalmClams_1 = require("../concrete/CalmClams");
var Codeonavirus_1 = require("../concrete/Codeonavirus");
var ExceptionalCoders_1 = require("../concrete/ExceptionalCoders");
var FlyingSplaucers_1 = require("../concrete/FlyingSplaucers");
var Fm_1 = require("../concrete/Fm");
var GnarlyNarwhals_1 = require("../concrete/GnarlyNarwhals");
var Medics_1 = require("../concrete/Medics");
var Webbscrapers_1 = require("../concrete/Webbscrapers");
var API_MAP = {
    'ApiDemic': ApiDemic_1["default"],
    'CalmClams': CalmClams_1["default"],
    'Codeonavirus': Codeonavirus_1["default"],
    'ExceptionalCoders': ExceptionalCoders_1["default"],
    'FlyingSplaucers': FlyingSplaucers_1["default"],
    'Fm': Fm_1["default"],
    'GnarlyNarwhals': GnarlyNarwhals_1["default"],
    'Medics': Medics_1["default"],
    'Webbscrapers': Webbscrapers_1["default"]
};
var articleStore = {
    /**
     * Returns an array of requests, one for each specified API
     * @param startDate - the start date in UTC time
     * @param endDate   - the end date in UTC time
     * @param keyTerms  - an array of key terms
     * @param location  - a location
     * @param apis      - an array of names of the APIs to be used.
     *                    Leave this blank to use all APIs.
     */
    createRequests: function (startDate, endDate, keyTerms, location, apis) {
        if (apis === void 0) { apis = []; }
        if (apis.length == 0) {
            apis = Object.keys(API_MAP);
        }
        return apis
            .filter(function (name) { return name in API_MAP; })
            .map(function (name) {
            return new ArticleRequest_1["default"](startDate, endDate, keyTerms, location, new API_MAP[name]());
        });
    }
};
exports["default"] = articleStore;
