"use strict";
// Standard location object
exports.__esModule = true;
var StandardLocation = /** @class */ (function () {
    function StandardLocation(country, location, geonamesId, googleId, coordinates) {
        if (geonamesId === void 0) { geonamesId = null; }
        if (googleId === void 0) { googleId = null; }
        if (coordinates === void 0) { coordinates = null; }
        this.country = country;
        this.location = location;
        this.geonamesId = geonamesId;
        this.googleId = googleId;
        this.coordinates = coordinates;
    }
    return StandardLocation;
}());
exports["default"] = StandardLocation;
