import { Component, OnInit } from "@angular/core";

import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";
import { DateFormatterService } from "../../../Services/date-formatter.service";
import { NgbModal, ModalDismissReasons } from "@ng-bootstrap/ng-bootstrap";

import { faMapMarkedAlt, faListAlt } from '@fortawesome/free-solid-svg-icons';
import { MapArticlesPopupComponent } from "../map-articles-popup/map-articles-popup.component";
import { AgmInfoWindow } from '@agm/core/directives/info-window';

import articleStore from "../../apis/articles/interfaces/articleStore";
import * as moment from "moment";
import StandardArticle from "../../types/StandardArticle";
import { isDefined } from "@ng-bootstrap/ng-bootstrap/util/util";
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import StandardReport from "../../types/StandardReport";

declare var google;
@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"],
})
export class MapComponent implements OnInit {
  // Icons
  faMapMarkedAlt = faMapMarkedAlt;
  faListAlt = faListAlt;
  selectedDiseases = [];
  masterMap: Map<string, StandardArticle[]> = new Map<string, StandardArticle[]>();
  map: Map<string, StandardArticle[]> = new Map<string, StandardArticle[]>();
  currentMarker;
  searchResult: StandardArticle[] = [];
  filterResult: StandardArticle[] = [];
  //infowindow = new google.maps.InfoWindow();
  markers: any[] = [];
  mapView = true;
  allDiseases = [];
  mapViewToggleState = true;
  dropdownSettings = {};
  infoWindowOpened: AgmInfoWindow = null;
  previous_info_window: AgmInfoWindow = null;

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
    if (sessionStorage.getItem("map") === null || JSON.parse(sessionStorage.getItem("map")).length == 0) {
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
      //console.log(this.map);
    } else {
      this.masterMap = new Map<string, StandardArticle[]>(JSON.parse(sessionStorage.getItem("map")));
      this.map = new Map<string, StandardArticle[]>(JSON.parse(sessionStorage.getItem("map")));
      this.setMarkers();
      this.getFilteredArticles();
      this.convertMapToArray();
      this.getAllDiseases();
    }

    this.articleService.currentStatus.subscribe(x => {
      if (x === true) {
        this.searchResult = [];
        this.selectedDiseases = [];
        this.filterResult = [];
        this.map = new Map<string, StandardArticle[]>();
        const request = this.articleService.currentRequest.subscribe(x => { this.getAllRequests(x) });
        this.articleService.modifyStatus(false);
        // this.articleService.updateListView(this.searchResult);
        this.infoWindowOpened = null;
        this.previous_info_window = null;
      } else {
        console.log("already loaded map search");
      }
    });
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item_id',
      textField: 'Filter',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 1,
      allowSearchFilter: true
    };
  }

  getAllDiseases() {
    this.allDiseases = [];
    this.searchResult.forEach(article => {
      article.reports.forEach(report => {
        report.diseases.forEach(disease => {
          if (!this.allDiseases.includes(disease)) {
            this.allDiseases.push(disease);
          }
        })
      })
    })
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
            const article = new StandardArticle(element.url, element.dateOfPublicationStr, element.headline, element.mainText, element.reports, element.teamName, element.source, element.id, element.extra);
            ret.push(article);
          });
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

  toggleMapView(mapView: boolean) {
    if (this.mapView !== mapView) {
      this.resetPopup();
    }
  }

  resetPopup() {
    this.infoWindowOpened = null;
    this.previous_info_window = null;
    this.mapView = !this.mapView;
  }

  async getAllRequests(articleRequests) {
    articleRequests.forEach((req) => {
      req.fetchAll().then((res) => {
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
                        this.masterMap.set(stringCoordinates, update);
                      } else {
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                        this.masterMap.set(stringCoordinates, update);
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
                        this.masterMap.set(stringCoordinates, update);
                      } else {
                        var update = this.map.get(stringCoordinates);
                        update.push(article);
                        this.map.set(stringCoordinates, update);
                        this.masterMap.set(stringCoordinates, update);
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
            this.masterMap.set(x, uniqueArray);
          }
        })
        sessionStorage.setItem("map", JSON.stringify(Array.from(this.map.entries())));
        this.getFilteredArticles();
        this.convertMapToArray();
        this.getAllDiseases();
      });
    });
  }

  convertMapToArray() {
    this.searchResult = [];
    Array.from(this.masterMap.keys()).forEach(x => {
      this.masterMap.get(x).forEach(res => {
        this.searchResult.push(res);
      })
    })
    this.searchResult = this.searchResult.filter((thing, index) => {
      const _thing = JSON.stringify(thing.headline) + JSON.stringify(thing.dateOfPublicationStr);
      return index === this.searchResult.findIndex(obj => {
        return JSON.stringify(obj.headline) + JSON.stringify(obj.dateOfPublicationStr) === _thing;
      });
    });
  }

  switchView() {
    this.mapView = !this.mapView;
  }

  onItemSelect(item: any) {
    console.log(item);
  }

  onSelectAll(items: any) {
    console.log(items);
  }

  FilterMasterMap() {
    this.infoWindowOpened = null;
    this.previous_info_window = null;
    if (this.selectedDiseases.length == 0) {
      console.log("nothing to filter");
    } else {
      let newMap = new Map<string, StandardArticle[]>();
      Array.from(this.masterMap.keys()).forEach(x => {
        newMap.set(x, []);
      })
      // go through each marker
      // check all the articles in each marker
      // check all the reports in the article
      // check all the diseases in the article
      // if match, add the marker + article
      Array.from(this.masterMap.keys()).forEach(x => {
        const articles = this.masterMap.get(x);
        articles.forEach(article => {
          const reports = article.reports;
          var found = false;
          reports.forEach(report => {
            const diseases = report.diseases;
            diseases.forEach(disease => {
              if (this.selectedDiseases.includes(disease) && found == false) {
                found = true;
                let articlesOnMarker = newMap.get(x);
                articlesOnMarker.push(article);
                newMap.set(x, articlesOnMarker);
              }
            })
          })
        })
      });

      Array.from(this.masterMap.keys()).forEach(x => {
        if (newMap.get(x).length == 0) {
          newMap.delete(x);
        }
      })
      this.map = newMap;
      this.getFilteredArticles();
      //console.log(this.filterResult);
      this.selectedDiseases = [];
      this.articleService.updateListView(this.filterResult);
      this.setMarkers();
    }
  }

  getFilteredArticles() {
    this.filterResult = [];
    Array.from(this.map.keys()).forEach(x => {
      this.map.get(x).forEach(res => {
        this.filterResult.push(res);
      })
    })
    this.filterResult = this.filterResult.filter((thing, index) => {
      const _thing = JSON.stringify(thing.headline) + JSON.stringify(thing.dateOfPublicationStr);
      return index === this.filterResult.findIndex(obj => {
        return JSON.stringify(obj.headline) + JSON.stringify(obj.dateOfPublicationStr) === _thing;
      });
    });
  }

  setMarkers() {
    let markerId = 1;
    this.markers = [];
    Array.from(this.map.keys()).forEach(x => {
      var latlongString = x.split("&");
      // if (!this.checkMarkerInMap(latlongString[0], latlongString[1])) {
      const marker = { lat: latlongString[0], lng: latlongString[1], alpha: 1, id: markerId };
      this.markers.push(marker);
      markerId++;
      // }
    });
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
