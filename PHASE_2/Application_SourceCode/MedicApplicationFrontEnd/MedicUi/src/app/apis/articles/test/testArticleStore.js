"use strict";
exports.__esModule = true;
var util = require("util");
var articleStore_1 = require("../interfaces/articleStore");
var requests = articleStore_1["default"].createRequests(
  new Date(2020, 0, 1, 0, 0, 0),
  new Date(2020, 3, 4, 0, 0, 0),
  ["infection", "outbreak"],
  "",
  ["Medics", "Codeonavirus"]
);
requests.forEach(function (req) {
  return req.fetchFirst().then(function (res) {
    return console.log(util.inspect(res, false, null, true));
  });
});
