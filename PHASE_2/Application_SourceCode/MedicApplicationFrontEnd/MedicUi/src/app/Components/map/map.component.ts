import { Component, OnInit } from "@angular/core";

import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";
import { DateFormatterService } from "../../../Services/date-formatter.service";
import { NgbModal, ModalDismissReasons } from "@ng-bootstrap/ng-bootstrap";

import { MapArticlesPopupComponent } from "../map-articles-popup/map-articles-popup.component";

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
  openedWindow: number = 0; // alternative: array of numbers

  constructor(
    private articleRetriever: ArticleRetrieverService,
    private locationRetriever: LocationMapperService,
    private dateFormatter: DateFormatterService,
    private modalService: NgbModal
  ) { }

  async ngOnInit() {
    // if (localStorage.getItem("map") === null || JSON.parse(localStorage.getItem("map")) == []) {
    await this.getAllRequests().then(() => {
      console.log("POOO");
      alert("DONE");
    });
    // } else {
    //   var x = await this.locationRetriever.convertGeoIdToLocation("204376").then(xx => console.log(xx));
    //   this.map = new Map<{ latitude: any; longtitude: any }, StandardArticle[]>(JSON.parse(localStorage.getItem("map")));
    //   let markerId = 1;
    //   Array.from(this.map.keys()).forEach(x => {
    //     const marker = { lat: x.latitude, lng: x.longtitude, alpha: 1, id: markerId };
    //     this.markers.push(marker);
    //     markerId++;
    //   })
    // }
  }

  openWindow(id, lat, long) {
    console.log(id, lat, long);
    if (id == this.openedWindow && this.openedWindow !== undefined) {
      this.openedWindow = -1;
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

    return new Promise(() => {
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
                const _thing = JSON.stringify(thing);
                return index === this.map.get(x).findIndex(obj => {
                  return JSON.stringify(obj) === _thing;
                });
              });
              this.map.set(x, uniqueArray);
              console.log("we in", uniqueArray);
            }
          })
          //console.log(this.map);
          localStorage.setItem("map", JSON.stringify(Array.from(this.map.entries())));
        });
      });
    });
  }

  getArticlesFromLatitudeLongtitude(latitude, longtitude) {
    const coordinates = {
      latitude: latitude.toString(),
      longtitude: longtitude.toString(),
    };
    for (const [key, value] of this.map.entries()) {
      var latlongString = key.split("&");
      if (latlongString[0] === latitude.toString() && latlongString[1] === longtitude.toString()) {
        return value;
      }
    }
  }

  checkMarkerInMap(latitude, longtitude) {
    this.markers.forEach(x => {
      if (x.latitude === latitude || x.longtitude === longtitude) {
        return true;
      }
    })

    return false;
  }

  close_window() {
    this.openedWindow = -1;
  }


  styles: any[] = [
    {
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#f5f5f5"
        }
      ]
    },
    {
      "elementType": "labels.icon",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    },
    {
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#616161"
        }
      ]
    },
    {
      "elementType": "labels.text.stroke",
      "stylers": [
        {
          "color": "#f5f5f5"
        }
      ]
    },
    {
      "featureType": "administrative.land_parcel",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#bdbdbd"
        }
      ]
    },
    {
      "featureType": "poi",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#eeeeee"
        }
      ]
    },
    {
      "featureType": "poi",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#757575"
        }
      ]
    },
    {
      "featureType": "poi.park",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#e5e5e5"
        }
      ]
    },
    {
      "featureType": "poi.park",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    },
    {
      "featureType": "road",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    },
    {
      "featureType": "road",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#ffffff"
        }
      ]
    },
    {
      "featureType": "road.arterial",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#757575"
        }
      ]
    },
    {
      "featureType": "road.highway",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#dadada"
        }
      ]
    },
    {
      "featureType": "road.highway",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#616161"
        }
      ]
    },
    {
      "featureType": "road.local",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    },
    {
      "featureType": "transit.line",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#e5e5e5"
        }
      ]
    },
    {
      "featureType": "transit.station",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#eeeeee"
        }
      ]
    },
    {
      "featureType": "water",
      "elementType": "geometry",
      "stylers": [
        {
          "color": "#c9c9c9"
        }
      ]
    },
    {
      "featureType": "water",
      "elementType": "labels.text.fill",
      "stylers": [
        {
          "color": "#9e9e9e"
        }
      ]
    }
  ];
}
