import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { HttpClient } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class LocationMapperService {
  constructor(private http: HttpClient) {}

  convertLocationToGeoLocation(country: string, location: string) {
    const searchParams = new HttpParams({
      fromString: "country=" + country + "&end_date=" + location,
    });

    // geonamesId: number;
    // googleId: string;
    // coordinates; // object with latitude and longitude fields
  }
}
