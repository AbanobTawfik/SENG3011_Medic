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
  private start;
  private timeline = [{}];
  private hoverIndex = 0;
  
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
    this.start = cases.start;
    this.updateTimeline();
    this.updatePopulation();
  }
  
  exposed = true;
  infectious = true;
  removed = true;
  recovered = true;
  hospitalised = true;
  deaths = true;
  
  population: number = 3000000;
  initial: number = 1;
  r0: number = 3;
  dIncubation: number = 5;
  dInfectious: number = 4;
  cfr: number = 10;
  dDeath: number = 12;
  dRecoveryMild: number = 11.1;
  dRecoverySevere: number = 28.6;
  pServere: number = 20;
  dHospitalLag: number = 5;
  r0ReductionPercent: number = 67;
  r0ReductionDay: number = 80;
  
  updateTimeline() {
    const seirModel = new SeirModel({
      r0: this.r0,
      dDeath: this.dDeath - this.dInfectious,
      dIncubation: this.dIncubation,
      dInfectious: this.dInfectious,
      dRecoveryMild: this.dRecoveryMild,
      dRecoveryServere: this.dRecoverySevere,
      dHospitalLag: this.dHospitalLag,
      cfr: this.cfr / 100,
      pServere: this.pServere / 100,
      dt: 1,
    });
    const timeline = seirModel.calculate({
      population: this.population,
      initiallyInfected: this.initial,
      r0ReductionPercent: this.r0ReductionPercent,
      r0ReductionDay: this.r0ReductionDay,
      days: 200,
    });
    const order = [
      i => timeline[i].exposed,
      i => timeline[i].infected,
      i => timeline[i].recovered,
      i => timeline[i].hospitalized,
      i => timeline[i].deaths
    ];
    order.forEach((group, order, a) =>
      this.totalChartData[a.length - order].data = timeline.map((e, i) => {
        let d = new Date(this.start);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: group(i)
        }
      })
    )
    this.timeline = timeline;
  }
  
  onChartHover(event) {
    this.hoverIndex = event.active[0]._index;
  }
  
  ngOnInit() {}
  
  sliderPop: number;
  textPop: string;
  
  onPopChange(event) {
    if (event.target.type == "range")
      this.population = Math.round(Math.pow(10, this.sliderPop));
    else
      this.population = parseInt(this.textPop.replace(/[^\d]/g, ""));
    if (isNaN(this.population))
      this.population = 100;
    this.updatePopulation();
  }
  updatePopulation() {
    const regex = /(\d+)(\d{3})/;
    this.textPop = this.population.toString().replace(/^\d+/, function(w) {
      while (regex.test(w))
        w = w.replace(regex, '$1,$2');
      return w;
    });
    this.sliderPop = Math.log(this.population) / Math.LN10;
  }
  
  peakDate: NgbDateStruct = {year: 2020, month: 4, day: 10};
  onDateChange() {
    const reduceDate = new Date(this.peakDate.year, this.peakDate.month - 1, this.peakDate.day);
    this.r0ReductionDay = Math.round(Math.abs((reduceDate.getTime() - (new Date(this.start)).getTime()) / (24*60*60*1000)));
    this.updateTimeline();
  }
  updateInput(input) {
    input.dispatchEvent(new Event("input"));
  }
  
  public totalChartData = [{
    label: 'Cumulative cases',
    data: [],
    fill: 'false',
    backgroundColor: 'transparent',
    borderColor: '#1976D2',
    pointBackgroundColor: '#1976D2',
    pointHoverBorderColor: '#1976D2',
    yAxisID: 'A'
  },
  {
    label: 'Deaths',
    data: [],
    type: 'bar',
    backgroundColor: '#E0E0E0'
  },
  {
    label: 'Hospitalised',
    data: [],
    type: 'bar',
    backgroundColor: '#81D4FA'
  },
  {
    label: 'Recovered',
    data: [],
    type: 'bar',
    backgroundColor: '#C5E1A5'
  },
  {
    label: 'Infectious',
    data: [],
    type: 'bar',
    backgroundColor: '#F48FB1'
  },
  {
    label: 'Exposed',
    data: [],
    type: 'bar',
    backgroundColor: '#FFCC80'
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
        stacked: true,
        type: 'time'
      }],
      yAxes: [{
        id: 'A',
        stacked: true,
        type: 'linear',
        position: 'left'/*,
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
        }*/
      }]
    },
    legend: {
      display: false
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
