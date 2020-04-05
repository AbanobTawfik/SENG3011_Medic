"use strict";
exports.__esModule = true;
var moment = require("moment");
function xsToDateRange(y1s, M1s, d1s, H1s, m1s, s1s, y2s, M2s, d2s, H2s, m2s, s2s) {
    var y1 = parseInt(y1s);
    var y2 = parseInt(y2s);
    var M1 = (M1s == 'xx' ? 1 : parseInt(M1s));
    var M2 = (M2s == 'xx' ? 12 : parseInt(M2s));
    var dm = daysInMonth(M2, y2);
    var d1 = (d1s == 'xx' ? 1 : parseInt(d1s));
    var d2 = (d2s == 'xx' ? dm : parseInt(d2s));
    var H1 = (H1s == 'xx' ? 0 : parseInt(H1s));
    var H2 = (H2s == 'xx' ? 23 : parseInt(H2s));
    var m1 = (m1s == 'xx' ? 0 : parseInt(m1s));
    var m2 = (m2s == 'xx' ? 59 : parseInt(m2s));
    var s1 = (s1s == 'xx' ? 0 : parseInt(s1s));
    var s2 = (s2s == 'xx' ? 59 : parseInt(s2s));
    return [
        moment.utc([y1, M1 - 1, d1, H1, m1, s1]),
        moment.utc([y2, M2 - 1, d2, H2, m2, s2]),
    ];
}
function daysInMonth(m, y) {
    switch (m) {
        case 1: return 31;
        case 2: return isLeapYear(y) ? 29 : 28;
        case 3: return 31;
        case 4: return 30;
        case 5: return 31;
        case 6: return 30;
        case 7: return 31;
        case 8: return 31;
        case 9: return 30;
        case 10: return 31;
        case 11: return 30;
        case 12: return 31;
        default: return 0;
    }
}
function isLeapYear(y) {
    return y % 400 == 0 || (y % 100 != 0 && y % 4 == 0);
}
function xsToDateFloor(ys, Ms, ds, Hs, ms, ss) {
    var y = parseInt(ys);
    var M = (Ms == 'xx' ? 1 : parseInt(Ms));
    var d = (ds == 'xx' ? 1 : parseInt(ds));
    var H = (Hs == 'xx' ? 0 : parseInt(Hs));
    var m = (ms == 'xx' ? 0 : parseInt(ms));
    var s = (ss == 'xx' ? 0 : parseInt(ss));
    return moment.utc([y, M - 1, d, H, m, s]);
}
////////////////////////////////////////////////////////////////////////
var dateUtils = {
    xStrToDateRange: function (str) {
        if (str == null)
            return null;
        var reExact = /(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;
        var reRange = /(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx) *to *(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;
        var match1 = str.match(reExact);
        var match2 = str.match(reRange);
        if (match2) {
            var y1 = match2[1];
            var M1 = match2[2];
            var d1 = match2[3];
            var H1 = match2[4];
            var m1 = match2[5];
            var s1 = match2[6];
            var y2 = match2[7];
            var M2 = match2[8];
            var d2 = match2[9];
            var H2 = match2[10];
            var m2 = match2[11];
            var s2 = match2[12];
            return xsToDateRange(y1, M1, d1, H1, m1, s1, y2, M2, d2, H2, m2, s2);
        }
        else if (match1) {
            var y1 = match1[1];
            var M1 = match1[2];
            var d1 = match1[3];
            var H1 = match1[4];
            var m1 = match1[5];
            var s1 = match1[6];
            return xsToDateRange(y1, M1, d1, H1, m1, s1, y1, M1, d1, H1, m1, s1);
        }
        else {
            return null;
        }
    },
    xStrToDateFloor: function (str) {
        if (str == null)
            return null;
        var reExact = /(\d{4})-(\d{2}|xx)-(\d{2}|xx) (\d{2}|xx):(\d{2}|xx):(\d{2}|xx)/;
        var match1 = str.match(reExact);
        if (match1) {
            var y = match1[1];
            var M = match1[2];
            var d = match1[3];
            var H = match1[4];
            var m = match1[5];
            var s = match1[6];
            return xsToDateFloor(y, M, d, H, m, s);
        }
        else {
            return null;
        }
    }
};
exports["default"] = dateUtils;
