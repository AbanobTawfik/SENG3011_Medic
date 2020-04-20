// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  GoogleApiKey: "AIzaSyCkEeVkb2etUdi93S5RWX5pZ9cUaoOsQ7Q",
  MedicEndPoint: "https://localhost:5003/api/Location/",
  OpenMapsEndPoint: "https://nominatim.openstreetmap.org/search",
  GeoNameUserName: "medics",
  GeoIdEndPoint:
    "http://api.geonames.org/get",
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
