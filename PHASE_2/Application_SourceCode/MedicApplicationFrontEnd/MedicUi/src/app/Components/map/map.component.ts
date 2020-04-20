import { Component, OnInit } from "@angular/core";
import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";
import { DateFormatterService } from "../../../Services/date-formatter.service";
import { NgbModal, ModalDismissReasons } from "@ng-bootstrap/ng-bootstrap";

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

  map: Map<{ latitude: any; longtitude: any }, StandardArticle[]> = new Map<{ latitude: any; longtitude: any }, StandardArticle[]>();
  currentMarker;
  //infowindow = new google.maps.InfoWindow();
  markers: any[] = [];
  openedWindow: number = 0; // alternative: array of numbers

  constructor(
    private articleRetriever: ArticleRetrieverService,
    private locationRetriever: LocationMapperService,
    private dateFormatter: DateFormatterService,
    private modalService: NgbModal
  ) { }

  async ngOnInit() {
    if (localStorage.getItem("map") === null || JSON.parse(localStorage.getItem("map")) == []) {
      await this.getAllRequests().then(() => {
        console.log("used map api");
      });
    } else {
      console.log("autoloaded")
      this.map = new Map<{ latitude: any; longtitude: any }, StandardArticle[]>(JSON.parse(localStorage.getItem("map")));
      let markerId = 1;
      Array.from(this.map.keys()).forEach(x => {
        const marker = { lat: x.latitude, lng: x.longtitude, alpha: 1, id: markerId };
        this.markers.push(marker);
        markerId++;
      })
    }
  }

  openWindow(id, lat, long) {
    console.log(id, lat, long);
    if (id == this.openedWindow && this.openedWindow !== undefined) {
      this.openedWindow = 0;
      return;
    }
    this.openedWindow = id; // alternative: push to array of numbers
  }

  isInfoWindowOpen(id) {
    return this.openedWindow == id; // alternative: check if id is in array
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: "modal-basic-title" });
  }

  async getAllRequests() {
    var currentdate = moment();
    var previousweek = currentdate.subtract(2, "w");
    const articleRequests = articleStore.createRequests(
      moment.utc([2020, 0, 1, 0, 0, 0]),
      moment.utc([2020, 1, 1, 0, 0, 0]),
      [],
      "",
      []
    );

    articleRequests.forEach((req) => {
      req.fetchAmount(10).then((res) => {
        res.forEach((article) => {
          article.reports.forEach((report) => {
            report.locations.forEach((location) => {
              if (location.geonamesId === null) {
                const coords = this.locationRetriever
                  .convertLocationToGeoLocation(
                    location.country,
                    location.location
                  )
                  .then((resultant) => {
                    const coordinates = {
                      latitude: resultant.latitude,
                      longtitude: resultant.longtitude,
                    };
                    if (coordinates !== undefined && !this.map.has(coordinates)) {
                      this.map.set(coordinates, []);
                      var update = this.map.get(coordinates);
                      update.push(article);
                      this.map.set(coordinates, update);
                    } else {
                      var update = this.map.get(coordinates);
                      update.push(article);
                      this.map.set(coordinates, update);
                    }
                  });
              }
              else {
                const coordsGeoId = this.locationRetriever
                  .convertGeoIdToLocation(location.geonamesId.toString())
                  .then((resultant) => {
                    const coordinates = {
                      latitude: resultant.latitude,
                      longtitude: resultant.longtitude,
                    };
                    if (coordinates !== undefined && !this.map.has(coordinates)) {
                      this.map.set(coordinates, []);
                      var update = this.map.get(coordinates);
                      update.push(article);
                      this.map.set(coordinates, update);
                    } else {
                      var update = this.map.get(coordinates);
                      update.push(article);
                      this.map.set(coordinates, update);
                    }
                  });
              }
            });
          });
        });
      }).then(() => {
        let markerId = 1;
        Array.from(this.map.keys()).forEach(x => {
          const marker = { lat: x.latitude, lng: x.longtitude, alpha: 1, id: markerId };
          this.markers.push(marker);
          markerId++;
        })
        console.log(this.map);
        localStorage.setItem("map", JSON.stringify(Array.from(this.map.entries())));
      });
    });
  }

  getArticlesFromLatitudeLongtitude(latitude, longtitude) {
    const coordinates = {
      latitude: latitude.toString(),
      longtitude: longtitude.toString(),
    };
    for (const [key, value] of this.map.entries()) {
      if (key.latitude === latitude && key.longtitude === longtitude) {
        return value;
      }
    }
  }
}
