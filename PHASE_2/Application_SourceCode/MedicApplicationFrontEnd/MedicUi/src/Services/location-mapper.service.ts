import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";
import { NgxXml2jsonService } from "ngx-xml2json";
import { delay } from "rxjs/internal/operators";
@Injectable({
  providedIn: "root",
})
export class LocationMapperService {
  latitude: any;
  longtitude: any;
  constructor(
    private http: HttpClient,
    private ngxXml2jsonService: NgxXml2jsonService
  ) {}

  async convertLocationToGeoLocation(country: string, location: string) {
    const stateOrCity =
      country.toLowerCase() == "United States".toLowerCase() ? "state" : "city";

    const searchParams = new HttpParams({
      fromString:
        "country=" +
        country +
        "&" +
        stateOrCity +
        "=" +
        location +
        "&format=json",
    });
    const options = { params: searchParams };
    var response = await this.http
      .get<any>(environment.OpenMapsEndPoint, options)
      .pipe(delay(2000))
      .toPromise();
    if (response === null || response === [] || response.Length === 0 || response[0] == null) {
      return {};
    } else {
      this.latitude = response[0]["lat"];
      this.longtitude = response[0]["lon"];
      return { latitude: this.latitude, longtitude: this.longtitude };
    }
  }

  async sleep(msec) {
    return new Promise((resolve) => setTimeout(resolve, msec));
  }

  async convertGeoIdToLocation(geoId: string) {
    // ?geonameId=5551752&username=medics
    var response = await this.http
      .get(
        environment.GeoIdEndPoint +
          "?geonameId=" +
          geoId +
          "&username=" +
          environment.GeoNameUserName,
        { responseType: "text" }
      )
      .pipe(delay(2000))
      .toPromise();
    const parser = new DOMParser();
    const xml = parser.parseFromString(response, "text/xml");
    var obj = this.ngxXml2jsonService.xmlToJson(xml);
    console.log(obj);
    this.latitude = obj["geoname"]["lat"];
    this.longtitude = obj["geoname"]["lng"];
    return { latitude: this.latitude, longtitude: this.longtitude };
  }
}
