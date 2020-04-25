import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

import Medics from '../../apis/articles/concrete/Medics';
import SeirModel from 'seir';

@Component({
  selector: 'app-summary',
  templateUrl: './model.component.html',
  styleUrls: ['./model.component.scss']
})
export class ModelComponent implements OnInit {
  
  private loaded: boolean = false;
  private error;
  private url;
  private article;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {
    route.queryParams.subscribe(params => this.loadArticle(params['article']));
    const state = this.router.getCurrentNavigation().extras.state;
    this.article = state ? state.article : {headline:'Epidemic Modelling'};
  }
  
  loadArticle(url: string) {
    this.url = url;
    this.http.get(
      (new Medics()).url + "/api/Reports/GetCases?url=" + url, {
        headers: new HttpHeaders({
          "Content-Type": "application/json"
        }),
        observe: 'response'
      }
    )
    .subscribe(response => {
      if (response.status == 204)
        this.error = 'No data available for';
      else if (response.status != 200)
        this.error = 'Unexpected ' + response.status + ' error for';
      else {
        this.loaded = true;
        this.loadInitial(response.body);
      }
    }, err => {
      console.log(err);
    });
  }
  
  loadInitial(cases) {
    console.log(cases);
    let total = 0;
    this.totalChartData[0].data = cases.data.map((n, i) =>
      {
        let d = new Date(cases.start);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: total += n
        }
      }
    );
    this.dailyChartData[0].data = cases.data.map((n, i) =>
      {
        let d = new Date(cases.start);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: n
        }
      }
    );
  }
  
  ngOnInit() {}
  
  inputR: number;
  peakDate: NgbDateStruct;
  
  onDateChange() {
    return;
  }
  
  updateInput(input) {
    input.dispatchEvent(new Event("input"));
  }
  
  public totalChartData = [{
    data: [],
    fill: 'false',
    label: 'Cumulative cases',
    backgroundColor: 'transparent',
    borderColor: '#1976D2',
    pointBackgroundColor: '#1976D2',
    pointHoverBorderColor: '#1976D2',
    yAxisID: 'A'
  },
  {
    data: [],
    fill: 'false',
    label: 'Projection',
    backgroundColor: 'transparent',
    borderColor: '#9E9E9E',
    borderDash: [5],
    pointBackgroundColor: '#9E9E9E',
    pointHoverBorderColor: '#9E9E9E',
    pointRadius: 0,
    yAxisID: 'A'
  },
  {
    data: [],
    label: 'Upper',
    type: 'line',
    fill: 'false',
    backgroundColor: 'rgb(158, 158, 158, 0.5)',
    borderColor: 'transparent',
    pointRadius: 0,
    yAxisID: 'A'
  },
  {
    data: [],
    label: 'Lower',
    type: 'line',
    fill: '-1',
    backgroundColor: 'rgb(158, 158, 158, 0.5)',
    borderColor: 'transparent',
    pointRadius: 0,
    yAxisID: 'A'
  }];
  public totalChartOptions = {
    scales: {
      xAxes: [{
        type: 'time'
      }],
      yAxes: [{
        id: 'A',
        type: 'logarithmic',
        position: 'left',
        scaleLabel: {
          display: true,
          labelString: 'Total cases'
        },
        ticks: {
          min: 1,
          max: 100000000,
          callback: function (value, index, values) {
            if (value < 1000)
              return value.toString();
            else if (value < 1000000)
              return value.toString().slice(0, -3) + "K";
            else
              return value.toString().slice(0, -6) + "M";
          }
        },
        afterBuildTicks: function (chartObj) {
          chartObj.ticks = [];
          chartObj.ticks.push(1);
          chartObj.ticks.push(10);
          chartObj.ticks.push(100);
          chartObj.ticks.push(1000);
          chartObj.ticks.push(10000);
          chartObj.ticks.push(100000);
          chartObj.ticks.push(1000000);
          chartObj.ticks.push(10000000);
          chartObj.ticks.push(100000000);
        }
      }]
    }
  }
  
  public dailyChartData = [{
    data: [],
    fill: 'false',
    label: 'Daily cases',
    backgroundColor: 'transparent',
    borderColor: '#4CAF50',
    pointBackgroundColor: '#4CAF50',
    pointHoverBorderColor: '#4CAF50',
    yAxisID: 'A'
  },
  {
    data: [],
    fill: 'false',
    label: 'Projection',
    backgroundColor: 'transparent',
    borderColor: '#9E9E9E',
    borderDash: [5],
    pointBackgroundColor: '#9E9E9E',
    pointHoverBorderColor: '#9E9E9E',
    pointRadius: 0,
    yAxisID: 'A'
  },
  {
    data: [],
    label: 'Upper',
    type: 'line',
    fill: 'false',
    backgroundColor: 'rgb(158, 158, 158, 0.5)',
    borderColor: 'transparent',
    pointRadius: 0,
    yAxisID: 'A'
  },
  {
    data: [],
    label: 'Lower',
    type: 'line',
    fill: '-1',
    backgroundColor: 'rgb(158, 158, 158, 0.5)',
    borderColor: 'transparent',
    pointRadius: 0,
    yAxisID: 'A'
  }];
  public dailyChartOptions = {
    scales: {
      xAxes: [{
        type: 'time'
      }],
      yAxes: [{
        id: 'A',
        type: 'logarithmic',
        position: 'left',
        scaleLabel: {
          display: true,
          labelString: 'New cases'
        },
        ticks: {
          min: 1,
          max: 10000000,
          callback: function (value, index, values) {
            if (value < 1000)
              return value.toString();
            else if (value < 1000000)
              return value.toString().slice(0, -3) + "K";
            else
              return value.toString().slice(0, -6) + "M";
          }
        },
        afterBuildTicks: function (chartObj) {
          chartObj.ticks = [];
          chartObj.ticks.push(1);
          chartObj.ticks.push(10);
          chartObj.ticks.push(100);
          chartObj.ticks.push(1000);
          chartObj.ticks.push(10000);
          chartObj.ticks.push(100000);
          chartObj.ticks.push(1000000);
          chartObj.ticks.push(10000000);
        }
      }]
    }
  }
}
