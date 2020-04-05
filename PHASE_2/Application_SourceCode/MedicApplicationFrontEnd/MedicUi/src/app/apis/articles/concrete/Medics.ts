// CDC
// https://seng3011medics.azurewebsites.net/swagger/index.html

import * as util from "util";

import * as moment from "moment";

import StandardArticle from "../../../types/StandardArticle";
import StandardLocation from "../../../types/StandardLocation";
import StandardReport from "../../../types/StandardReport";
import ArticleApi from "../base/ArticleApi";

class Medics extends ArticleApi {
  constructor() {
    super("https://localhost:5003", "/api/Reports/GetArticles");
    this.name = "Medics";

    this.limit = 50;
  }

  ////////////////////////////////////////////////////////////////////
  // Building a Request

  public makeQueryString(
    startDate: moment.Moment,
    endDate: moment.Moment,
    keyTerms: string[],
    location: string,
    pageNumber: number
  ): string {
    let query =
      `?start_date=${this.startDateValue(startDate)}` +
      `&end_date=${this.endDateValue(endDate)}`;

    if (keyTerms && keyTerms.length > 0) {
      query += `&key_terms=${this.keyTermsValue(keyTerms)}`;
    }

    if (location) {
      query += `&location=${this.locationValue(location)}`;
    }

    query += `&max=${this.limit}`;
    query += `&offset=${this.limit * pageNumber}`;

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
    const geonamesId = resLocation["geonames_id"];

    // TODO: Find more robust way to get the city
    const location2 = location.replace(/,.*/, "");

    return new StandardLocation(country, location2, geonamesId);
  }

  ////////////////////////////////////////////////////////////////////
}

export default Medics;
