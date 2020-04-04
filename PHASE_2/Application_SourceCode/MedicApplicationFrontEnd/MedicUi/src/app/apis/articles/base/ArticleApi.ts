import * as moment from "moment";
import * as fetch from "node-fetch";

abstract class ArticleApi {
  url: string;
  endpoint: string;

  constructor(url: string, endpoint: string) {
    this.url = url;
    this.endpoint = endpoint;
  }

  ////////////////////////////////////////////////////////////////////
  /**
   *
   * @param start_date - start date in local time
   * @param end_date   - end date in local time
   * @param key_terms  - list of key terms
   * @param location   - location
   */
  async fetchArticles(
    startDate: Date,
    endDate: Date,
    keyTerms: string[] = [],
    location: string = ""
  ) {
    if (!location) location = "";

    let method = this.makeMethod();

    let query = this.makeQueryString(startDate, endDate, keyTerms, location);

    console.log(query);

    let body = this.makeBody(startDate, endDate, keyTerms, location);

    console.log(body);

    let headers = this.makeHeaders();

    let request = `${this.url}${this.endpoint}${query}`;

    let response = await fetch(request, {
      method: method,
      ...body,
      headers: {
        Accept: "application/json",
        ...headers,
      },
    });

    let responseJson = await response.json();
    return this.processResponse(responseJson);
  }

  ////////////////////////////////////////////////////////////////////
  // Building a Request
  // Override to replace default

  public makeMethod(): string {
    return "GET";
  }

  public abstract makeQueryString(
    startDate: Date,
    endDate: Date,
    keyTerms: string[],
    location: string
  ): string;

  public makeBody(
    startDate: Date,
    endDate: Date,
    keyTerms: string[],
    location: string
  ): object {
    return {};
  }

  public makeHeaders(): object {
    return {};
  }

  ////////////////////////////////////////////////////////////////////
  // Parameter Values

  public startDateValue(date: Date): string {
    return moment(date).format("YYYY-MM-DDTHH:mm:ss");
  }

  public endDateValue(date: Date): string {
    return moment(date).format("YYYY-MM-DDTHH:mm:ss");
  }

  public keyTermsValue(keyTerms: string[]): string {
    return keyTerms.join(",");
  }

  public locationValue(location: string): string {
    return location;
  }

  ////////////////////////////////////////////////////////////////////
  // Post-Processing

  public abstract processResponse(responseJson);

  ////////////////////////////////////////////////////////////////////
}

export default ArticleApi;
