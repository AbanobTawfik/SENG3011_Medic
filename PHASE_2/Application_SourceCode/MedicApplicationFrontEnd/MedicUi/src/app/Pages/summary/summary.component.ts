import { Component, OnInit } from '@angular/core';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  
  constructor() { }
  
  private cdcData = "1	1	2	2	5	5	5	5	5	7	8	8	11	11	11	11	11	11	11	11	12	12	13	13	13	13	13	13	13	13	15	15	15	15	15	15	16	16	24	30	53	80	98	164	214	279	423	647	937	1215	1629	1896	2234	3487	4226	7038	10442	15219	18747	24583	33404	44183	54453	68440	85356	103321	122653	140904	163539	186101	213144	239279	277205";
  private cdcDate = new Date("2020-01-21");
  public lineChartLabels = [new Date("2020-01-20").toLocaleString(), new Date("2020-05-04").toLocaleString()];
  public lineChartType = 'line';
  public lineChartLegend = true;
  public lineChartData = [{
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
  public lineChartOptions = {
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
  
  public line2ChartLabels = [new Date("2020-01-20").toLocaleString(), new Date("2020-05-04").toLocaleString()];
  public line2ChartType = 'line';
  public line2ChartLegend = true;
  public line2ChartData = [{
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
  public line2ChartOptions = {
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
  
  modelPeak: NgbDateStruct;
  inputR: number;
  inputPeak: NgbDateStruct;

  ngOnInit() {
    let totals = this.cdcData.split("\t");
    this.lineChartData[0].data = totals.map((n, i) =>
      {
        let d = new Date(this.cdcDate);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: n
        }
      }
    );
    this.line2ChartData[0].data = totals.map((n, i) =>
      {
        let d = new Date(this.cdcDate);
        d.setDate(d.getDate() + i);
        return {
          t: d,
          y: Number(n) - ((i > 0) ? Number(totals[i-1]) : 0)
        }
      }
    );
    this.modelPeak = this.inputPeak = { day: 12, month: 4, year: 2020 };
    this.inputR = 2;
    this.predict();
  }

  dateChanged(date) {
    this.inputPeak = date;
    this.predict();
  }
  
  predict() {
    console.log(this.inputR + " " + this.inputPeak);
    let totals = this.cdcData.split("\t");
    let rate = Number(totals[totals.length - 2]) / Number(totals[totals.length - 3]);
    let curr = new Date("2020-04-02");
    let total = Number(totals[totals.length - 1]);
    let slow = new Date(this.inputPeak.year + "-" + this.inputPeak.month + "-" + this.inputPeak.day);
    let end = Math.round((slow.getTime() - curr.getTime()) / (24*60*60*1000));
    this.lineChartData[1].data = [];
    this.lineChartData[2].data = [];
    this.lineChartData[3].data = [];
    this.line2ChartData[1].data = [];
    this.line2ChartData[2].data = [];
    this.line2ChartData[3].data = [];
    for (let i = 0; i < 33; i++) {
      this.lineChartData[1].data.push({ t: new Date(curr), y: Math.floor(total) });
      this.lineChartData[2].data.push({ t: new Date(curr), y: Math.floor(total * (i / 15 + 1)) });
      this.lineChartData[3].data.push({ t: new Date(curr), y: Math.floor(total * (1 - i / 35)) });
      this.line2ChartData[1].data.push({ t: new Date(curr), y: Math.floor(total * (rate - 1)) });
      this.line2ChartData[2].data.push({ t: new Date(curr), y: Math.floor(total * (rate - 1) * (i / 15 + 1)) });
      this.line2ChartData[3].data.push({ t: new Date(curr), y: Math.floor(total * (rate - 1) * (1 - i / 35)) });
      curr.setDate(curr.getDate() + 1);
      total *= rate;
      if (i >= end) {
        rate = (rate - 1) * 0.9 * (0.5 + (this.inputR - 1) * 0.5) + 1;
      }
    }
  }
}
