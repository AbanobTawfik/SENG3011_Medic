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
  ) { }

  async convertLocationToGeoLocation(country: string, location: string) {
    var checkResponse = await this.CheckTheDbFirst(null, country, location).toPromise();
    if (!checkResponse["error"]) {
      if (checkResponse["longtitude"] && checkResponse["latitude"]) {
        console.log("returned from our api");
        return { latitude: parseFloat(checkResponse["latitude"]), longtitude: parseFloat(checkResponse["longtitude"]) };
      }
    }else{
      return this.GetLocationToGeoLocation(country, location);
    }
  }

  async GetLocationToGeoLocation(country: string, location: string) {
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
      await this.AddToOurDb(this.latitude, this.longtitude, null, country, location);
      return { latitude: this.latitude, longtitude: this.longtitude };
    }
  }

  async sleep(msec) {
    return new Promise((resolve) => setTimeout(resolve, msec));
  }

  async convertGeoIdToLocation(geoId: string) {
    var checkResponse = await this.CheckTheDbFirst(geoId, null, null).toPromise();
    if (!checkResponse["error"]) {
      if (checkResponse["longtitude"] && checkResponse["latitude"]) {
        console.log("returned from our api");
        return { latitude: parseFloat(checkResponse["latitude"]), longtitude: parseFloat(checkResponse["longtitude"]) };
      }
    }else{
      return this.GetGeoIdFromLocation(geoId);
    }
  }

  async GetGeoIdFromLocation(geoId: string) {
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
    this.latitude = obj["geoname"]["lat"];
    this.longtitude = obj["geoname"]["lng"];
    await this.AddToOurDb(this.latitude, this.longtitude, geoId, null, null);
    return { latitude: this.latitude, longtitude: this.longtitude };
  }

  CheckTheDbFirst(GeoId?: string, Country?: string, Location?: string) {
    const httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
      })
    };
    if (GeoId == null) {
      const place = { "Country": Country, "Location": Location };
      return this.http.post(environment.MedicEndPoint + "GeoName", place, httpOptions);
    } else {
      const place = { "GeoId": GeoId };
      return this.http.post(environment.MedicEndPoint + "GeoId", place, httpOptions);
    }

  }

  async AddToOurDb(latitude: string, longtitude: string, GeoId?: string, Country?: string, Location?: string) {
    const httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
      })
    };
    const place = { "Latitude": latitude, "Longtitude": longtitude, "GeoId": GeoId, "Country": Country, "Location": Location };
    await this.http
      .post(environment.MedicEndPoint + "AddLocation", place, httpOptions).toPromise()
  }
}
