import { Component, OnInit } from "@angular/core";
import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";
import { DateFormatterService } from "../../../Services/date-formatter.service";
import { NgbModal, ModalDismissReasons } from "@ng-bootstrap/ng-bootstrap";

import articleStore from "../../apis/articles/interfaces/articleStore";
import * as moment from "moment";
import StandardArticle from "../../types/StandardArticle";
declare var google;
@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"],
})
export class MapComponent implements OnInit {
  map: Map<{ latitude: any; longtitude: any }, StandardArticle[]>;
  currentMarker;
  constructor(
    private articleRetriever: ArticleRetrieverService,
    private locationRetriever: LocationMapperService,
    private dateFormatter: DateFormatterService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    // var currentdate = moment();
    // var previousweek = currentdate.subtract(2, "w");
    // const articleRequests = articleStore.createRequests(
    //   moment.utc([2020, 0, 1, 0, 0, 0]),
    //   moment.utc([2020, 1, 1, 0, 0, 0]),
    //   [],
    //   "",
    //   []
    // );
    // articleRequests.forEach((req) => {
    //   req.fetchAmount(10).then((res) => {
    //     res.forEach((article) => {
    //       article.reports.forEach((report) => {
    //         report.locations.forEach((location) => {
    //           if (location.geonamesId === null) {
    //             const coords = this.locationRetriever
    //               .convertLocationToGeoLocation(
    //                 location.country,
    //                 location.location
    //               )
    //               .then((resultant) => {
    //                 const coordinates = {
    //                   latitude: resultant.latitude,
    //                   longtitude: resultant.longtitude,
    //                 };
    //                 console.log(coordinates);
    //                 if (!this.map.has(coordinates)) {
    //                   this.map.set(coordinates, []);
    //                   var update = this.map.get(coordinates);
    //                   update.push(article);
    //                   this.map.set(coordinates, update);
    //                 } else {
    //                   var update = this.map.get(coordinates);
    //                   update.push(article);
    //                   this.map.set(coordinates, update);
    //                 }
    //               });
    //           } else {
    //             const coordsGeoId = this.locationRetriever
    //               .convertGeoIdToLocation(location.geonamesId.toString())
    //               .then((resultant) => {
    //                 const coordinates = {
    //                   latitude: resultant.latitude,
    //                   longtitude: resultant.longtitude,
    //                 };
    //                 console.log(coordinates);
    //                 if (!this.map.has(coordinates)) {
    //                   this.map.set(coordinates, []);
    //                   var update = this.map.get(coordinates);
    //                   update.push(article);
    //                   this.map.set(coordinates, update);
    //                 } else {
    //                   var update = this.map.get(coordinates);
    //                   update.push(article);
    //                   this.map.set(coordinates, update);
    //                 }
    //               });
    //           }
    //         });
    //       });
    //     });
    //   });
    // });
    // console.log("END END NED");
    // console.log(this.map);
  }
  //infowindow = new google.maps.InfoWindow();
  markers = [
    // These are all just random coordinates from https://www.random.org/geographic-coordinates/
    { lat: 22.33159, lng: 105.63233, alpha: 1, id: 3 },
    { lat: 7.92658, lng: -12.05228, alpha: 1, id: 4 },
    { lat: 48.75606, lng: -118.859, alpha: 1, id: 5 },
    { lat: 5.19334, lng: -67.03352, alpha: 1, id: 6 },
    { lat: 12.09407, lng: 26.31618, alpha: 1, id: 8 },
    { lat: 47.92393, lng: 78.58339, alpha: 1, id: 9 },
  ];

  clickedMarker() {
    var infowindow = new google.maps.InfoWindow({
      content: "poooop",
    });
    console.log(infowindow);
    infowindow.open();
  }
  openedWindow: number = 0; // alternative: array of numbers

  openWindow(id) {
    console.log(id);
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
  
  expandInput(event) {
    event.target.parentNode.getElementsByTagName("span")[0].innerText = event.target.value;
  }
  
  onFromSelect(event) {
    document.getElementById("from").dispatchEvent(new Event("input"));
  }
  
  onToSelect(event) {
    document.getElementById("to").dispatchEvent(new Event("input"));
  }
  
  change() {
    console.log("change");
  }
}
