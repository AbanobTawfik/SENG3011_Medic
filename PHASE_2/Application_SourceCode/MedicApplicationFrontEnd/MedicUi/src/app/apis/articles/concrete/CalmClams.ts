// Global Incident Map

import * as util from "util";

import StandardArticle from "../../../types/StandardArticle";
import StandardLocation from "../../../types/StandardLocation";
import StandardReport from "../../../types/StandardReport";
import ArticleApi from "../base/ArticleApi";

class CalmClams extends ArticleApi {
  constructor() {
    super("http://calmclams.appspot.com", "/disease_reports");
  }

  ////////////////////////////////////////////////////////////////////
  // Building a Request

  public makeQueryString(
    startDate: Date,
    endDate: Date,
    keyTerms: string[],
    location: string
  ): string {
    let query =
      `?start_date=${this.startDateValue(startDate)}` +
      `&end_date=${this.endDateValue(endDate)}`;

    if (keyTerms && keyTerms.length > 0) {
      query += `&keyterms=${this.keyTermsValue(keyTerms)}`;
    }

    if (location) {
      query += `&location=${this.locationValue(location)}`;
    }

    return query;
  }

  ////////////////////////////////////////////////////////////////////
  // Post-Processing

  public processResponse(responseJson) {
    return this.getArticlesFromResponse(responseJson);
  }

  private getArticlesFromResponse(json) {
    // console.log(util.inspect(json, false, null, true));
    return json["articles"].map((resArticle) =>
      this.toStandardArticle(resArticle)
    );
  }

  private toStandardArticle(resArticle) {
    const url = resArticle["url"];
    const dateOfPublication = resArticle["date_of_publication"];
    const headline = resArticle["headline"];
    const mainText = resArticle["main_text"];
    const reports = resArticle["reports"].map((resReport) =>
      this.toStandardReport(resReport)
    );

    return new StandardArticle(
      url,
      dateOfPublication,
      headline,
      mainText,
      reports
    );
  }

  private toStandardReport(resReport) {
    const diseases = resReport["diseases"];
    const syndromes = resReport["syndromes"];
    const event_date = resReport["event_date"];
    const locations = resReport["locations"].map((resLocation) =>
      this.toStandardLocation(resLocation)
    );

    return new StandardReport(diseases, syndromes, event_date, locations);
  }

  private toStandardLocation(resLocation) {
    const country = resLocation["country"];
    const location = resLocation["location"];
    const coordinates = this.toCoordinates(resLocation["coords"]);

    return new StandardLocation(country, location, null, null, coordinates);
  }

  private toCoordinates(coordinatesString) {
    if (!coordinatesString) return null;
    if (coordinatesString.search(/, ?/) === -1) return null;

    const coordinates = coordinatesString.split(/, ?/);
    return {
      latitude: parseFloat(coordinates[0]),
      longitude: parseFloat(coordinates[1]),
    };
  }

  ////////////////////////////////////////////////////////////////////
}

export default CalmClams;
