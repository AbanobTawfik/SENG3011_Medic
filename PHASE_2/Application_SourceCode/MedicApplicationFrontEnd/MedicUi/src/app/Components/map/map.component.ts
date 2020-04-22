import { Component, OnInit } from "@angular/core";

import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";
import { DateFormatterService } from "../../../Services/date-formatter.service";
import { NgbModal, ModalDismissReasons } from "@ng-bootstrap/ng-bootstrap";

import { MapArticlesPopupComponent } from "../map-articles-popup/map-articles-popup.component";
import { AgmInfoWindow } from '@agm/core/directives/info-window';

import articleStore from "../../apis/articles/interfaces/articleStore";
import * as moment from "moment";
import StandardArticle from "../../types/StandardArticle";
import { isDefined } from "@ng-bootstrap/ng-bootstrap/util/util";
declare var google;
@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"],
})
export class MapComponent implements OnInit {

  map: Map<string, StandardArticle[]> = new Map<string, StandardArticle[]>();
  currentMarker;
  //infowindow = new google.maps.InfoWindow();
  markers: any[] = [];
  mapLoaded = false;

  infoWindowOpened: AgmInfoWindow = null
  previous_info_window: AgmInfoWindow = null

  constructor(
    private articleService: ArticleRetrieverService,
    private locationRetriever: LocationMapperService,
    private dateFormatter: DateFormatterService,
    private modalService: NgbModal
  ) {
    this.articleService.currentStatus.subscribe(x => { console.log(x) });
    this.articleService.modifyStatus(false);
  }

  ngOnInit() {
    if (sessionStorage.getItem("map") === null || JSON.parse(sessionStorage.getItem("map")) == []) {
      var currentdate = moment();
      var previousweek = currentdate.subtract(2, "w");
      const articleRequests = articleStore.createRequests(
        moment.utc([2020, 0, 1, 0, 0, 0]),
        moment.utc([2020, 1, 1, 0, 0, 0]),
        [],
        "",
        []
      );
      this.getAllRequests(articleRequests).then(() => {
      });
    } else {
      this.map = new Map<string, StandardArticle[]>(JSON.parse(sessionStorage.getItem("map")));
      let markerId = 1;
      this.markers = [];
      Array.from(this.map.keys()).forEach(x => {
        var latlongString = x.split("&");
        // if (!this.checkMarkerInMap(latlongString[0], latlongString[1])) {
        const marker = { lat: latlongString[0], lng: latlongString[1], alpha: 1, id: markerId };
        this.markers.push(marker);
        markerId++;
        // }
      })
      console.log(this.map);
    }

    this.articleService.currentStatus.subscribe(x => {
      if(x === true){
        const request = this.articleService.currentRequest.subscribe(x => {this.getAllRequests(x)});
        this.articleService.modifyStatus(false);
      }else{
        console.log("already loaded map search");
      }

    });
  }

  close_window() {
    if (this.infoWindowOpened != null) {
      this.infoWindowOpened.close()
    }
  }

  select_marker(infoWindow) {
    if (this.infoWindowOpened == infoWindow) {
      this.infoWindowOpened.close();
      return;
    }

    if (this.infoWindowOpened !== null && this.infoWindowOpened !== undefined) {
      this.infoWindowOpened.close();
    }
    this.infoWindowOpened = infoWindow;
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: "modal-basic-title" });
  }

  getArticlesFromLatitudeLongtitude(latitude, longtitude) {
    if (!latitude || !longtitude) {
      return;
    }
    const coordinates = {
      latitude: latitude.toString(),
      longtitude: longtitude.toString(),
    };
    for (const [key, value] of this.map.entries()) {
      var latlongString = key.split("&");
      if (latlongString[0] === latitude.toString() && latlongString[1] === longtitude.toString()) {
        if (sessionStorage.getItem("map") === null) {
          return value;
        } else {
          let ret: StandardArticle[] = [];
          value.forEach(element => {
            const article = new StandardArticle(element.url, element.dateOfPublicationStr, element.headline, element.mainText, element.reports, element.teamName, element.id, element.extra);
            ret.push(article);
          });
          //console.log(ret);
          return ret;
        }
      }
    }
  }

  searchRequest(articleRequests) {
    this.getAllRequests(articleRequests).then(() => {
    });
  }

  checkMarkerInMap(latitude, longtitude) {
    this.markers.forEach(x => {
      if (x.latitude === latitude || x.longtitude === longtitude) {
        return true;
      }
    })

    return false;
  }

  async getAllRequests(articleRequests) {
    articleRequests.forEach((req) => {
      req.fetchAmount(10).then((res) => {
        res.forEach(async (article) => {
          await article.reports.forEach(async (report) => {
            await report.locations.forEach(async (location) => {
              if (!location.geonamesId && location.country && location.location) {
                await this.locationRetriever
                  .convertLocationToGeoLocation(
                    location.country,
                    location.location
                  )
                  .then((resultant) => {
                    const coordinates = {
                      latitude: resultant.latitude,
                      longtitude: resultant.longtitude,
                    };
                    if (coordinates.latitude && coordinates.longtitude) {
                      const stringCoordinates = coordinates.latitude.toString() + "-" + coordinates.longtitude.toString();
                      if (!this.map.has(stringCoordinates)) {
                        this.map.set(stringCoordinates, []);
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                      } else {
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                      }
                    }
                  });
              }
              else if (location.geonamesId) {
                await this.locationRetriever
                  .convertGeoIdToLocation(location.geonamesId.toString())
                  .then((resultant) => {
                    const coordinates = {
                      latitude: resultant.latitude,
                      longtitude: resultant.longtitude,
                    };
                    const stringCoordinates = coordinates.latitude.toString() + "&" + coordinates.longtitude.toString();
                    if (coordinates.latitude && coordinates.longtitude) {
                      if (!this.map.has(stringCoordinates)) {
                        this.map.set(stringCoordinates, []);
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                      } else {
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                      }
                    }
                  });
              }
            });
          });
        });
      }).then(() => {
        let markerId = 1;
        this.markers = [];
        Array.from(this.map.keys()).forEach(x => {
          var latlongString = x.split("&");
          if (!this.checkMarkerInMap(latlongString[0], latlongString[1])) {
            const marker = { lat: latlongString[0], lng: latlongString[1], alpha: 1, id: markerId };
            this.markers.push(marker);
            markerId++;
            const uniqueArray = this.map.get(x).filter((thing, index) => {
              const _thing = JSON.stringify(thing.headline) + JSON.stringify(thing.dateOfPublicationStr);
              return index === this.map.get(x).findIndex(obj => {
                return JSON.stringify(obj.headline) + JSON.stringify(obj.dateOfPublicationStr) === _thing;
              });
            });
            this.map.set(x, uniqueArray);
          }
        })
        sessionStorage.setItem("map", JSON.stringify(Array.from(this.map.entries())));
      });
    });
  }
}
