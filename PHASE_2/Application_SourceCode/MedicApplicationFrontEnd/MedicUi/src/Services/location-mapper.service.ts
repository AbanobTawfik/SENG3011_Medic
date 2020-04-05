import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";
import { NgxXml2jsonService } from "ngx-xml2json";

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
    await this.http
      .get<any>(environment.OpenMapsEndPoint, options)
      .subscribe((res) => {
        console.log(res);
      });
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
      .toPromise();
    const parser = new DOMParser();
    const xml = parser.parseFromString(response, "text/xml");
    var obj = this.ngxXml2jsonService.xmlToJson(xml);
    this.latitude = obj["geoname"]["lat"];
    this.longtitude = obj["geoname"]["lng"];
    console.log("print1 poopy");
    return { latitude: this.latitude, longtitude: this.longtitude };
  }
}
