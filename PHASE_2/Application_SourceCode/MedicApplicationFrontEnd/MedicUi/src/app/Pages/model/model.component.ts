import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { BaseChartDirective } from 'ng2-charts';

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
  private ongoing: boolean = true;
  private start;
  private predict: boolean = true;
  private loadPercent = 10;
  private timeline: any = [{}];
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
    let total = 0;
    let d;
    this.totalChartData[0].data = cases.data.map((n, i) =>
      {
        d = new Date(cases.start);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: total += n
        }
      }
    );
    let today = new Date();
    today.setDate(today.getDate() - 7);
    d = new Date(d);
    while (d < today) {
      this.totalChartData[0].data.push({t: new Date(d), y: total});
      d.setDate(d.getDate() + 1);
      this.ongoing = false;
    }
    this.start = cases.start;
    this.predictModel(cases.data, cases.deaths);
    this.updatePopulation();
  }
  
  population: number = 10000000;
  initial: number = 1;
  r0: number = 3; // 1.55
  dIncubation: number = 5;
  dInfectious: number = 4;
  cfr: number = 10; // 0
  dDeath: number = 14;
  dRecoveryMild: number = 11.1;
  dRecoverySevere: number = 28.6;
  pServere: number = 20;
  dHospitalLag: number = 5;
  r0ReductionPercent: number = 70; // 100
  r0ReductionDay: number = 78; // 2020-02-18
  
  bestError: number;
  bestDay: number;
  bestR0: number;
  
  predictModel(data, deaths) {
    this.cfr = 10;
    this.r0ReductionPercent = 100;
    this.bestError = 2000000000;
    this.bestDay = 30;
    this.bestR0 = 1;
    this.predictDay(40, data, deaths);
  }
  
  predictDay(day, data, deaths) {
    this.r0ReductionDay = day;
    this.loadPercent = (this.r0ReductionDay - 30) * 2;
    let error, total;
    for (this.r0 = 1.4; this.r0 < 4.2; this.r0 += 0.2) {
      this.updateTimeline();
      error = total = 0;
      for (let i = 0; i < data.length; i++) {
        total += data[i];
        error += Math.abs(total - this.timeline[i].totalInfected);
        if (error > this.bestError)
          break;
      }
      if (error < this.bestError) {
        this.bestDay = this.r0ReductionDay;
        this.bestR0 = this.r0;
        this.bestError = error;
      }
    }
    if (this.r0ReductionDay < 80) {
      setTimeout((() => this.predictDay(day + 2, data, deaths)).bind(this));
    } else {
      if (deaths == 0)
        this.cfr = 0; 
      else
        this.r0ReductionPercent = 70;
      this.r0ReductionDay = this.bestDay;
      this.r0 = this.bestR0;
      this.predict = false;
      this.updateTimeline();
    }
  }
  
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
    this.timeline = timeline;
    if (this.predict)
      return;
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
  }
  
  @ViewChild(BaseChartDirective, {static: false})
  chart: BaseChartDirective;
  exposed = true;
  infectious = true;
  removed = true;
  recovered = true;
  hospitalised = true;
  deaths = true;
  
  updateHidden(index, state) {
    this.totalChartData[index].hidden = state;
    this.chart.update();
  }
  updateHiddenRemoved(state) {
    this.recovered = this.hospitalised = this.deaths = !state;
    this.totalChartData[1].hidden = this.totalChartData[2].hidden = this.totalChartData[3].hidden = state;
    this.chart.update();
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
  
  peakDate: NgbDateStruct = {year: 2020, month: 4, day: 8};
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
    backgroundColor: '#E0E0E0',
    hidden: !this.deaths
  },
  {
    label: 'Hospitalised',
    data: [],
    type: 'bar',
    backgroundColor: '#81D4FA',
    hidden: !this.hospitalised
  },
  {
    label: 'Recovered',
    data: [],
    type: 'bar',
    backgroundColor: '#C5E1A5',
    hidden: !this.recovered
  },
  {
    label: 'Infectious',
    data: [],
    type: 'bar',
    backgroundColor: '#F48FB1',
    hidden: !this.infectious
  },
  {
    label: 'Exposed',
    data: [],
    type: 'bar',
    backgroundColor: '#FFCC80',
    hidden: !this.exposed
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
}
