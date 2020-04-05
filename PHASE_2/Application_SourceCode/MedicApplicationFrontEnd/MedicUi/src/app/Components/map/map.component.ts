import { Component, OnInit } from "@angular/core";
import { ArticleRetrieverService } from "../../../Services/article-retriever.service";
import { LocationMapperService } from "../../../Services/location-mapper.service";

declare var google;
@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"],
})
export class MapComponent implements OnInit {
  lat = 43.879078;
  lng = -103.4615581;
  currentMarker;
  constructor(
    private articleRetriever: ArticleRetrieverService,
    private locationRetriever: LocationMapperService
  ) {}

  ngOnInit() {
    var x = this.locationRetriever
      .convertLocationToGeoLocation("China", "Wuhan")
      .then((res) => console.log(res));
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
}
