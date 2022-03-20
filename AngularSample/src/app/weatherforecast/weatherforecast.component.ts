import { Component, OnInit } from "@angular/core";
import { HttpClient, HttpRequest } from "@angular/common/http";
import { Hero } from "../hero";


interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: "app-weatherforecast",
  templateUrl: "./weatherforecast.component.html",
  styleUrls: ["./weatherforecast.component.css"]
})
export class WeatherforecastComponent implements OnInit {
  public forecasts?: WeatherForecast[];

  public heroes?: Hero[];

  constructor(http: HttpClient) {
    const re = http.get<WeatherForecast[]>("api/v1/weatherforecast");
    re.subscribe((x: any) => this.forecasts = x,
      (err: any) => {console.error(err)},
      () => { });
  }

  ngOnInit(): void {
  }

  title = "AngularAppです";
  name = "なまえです";
  inner = `<div>innerHTMLを使った部分</div>`;

  message1 = "押しちゃった(⋈◍＞◡＜◍)。✧♡";
  message2 = "だめって言ったのに(T . T)💦";
  count = 0;

  alertMethod = () => {
    if(this.count % 2 === 0)
      alert(this.message1);
    else
      alert(this.message2);

    this.count++;
  }

  inputValue: string = "";
  inputValueLength: number = this.inputValue.length;
  setInputValue = (e: any) => {
    this.inputValue = e.target.value;
    this.inputValueLength = this.inputValue.length;
  }

  inputValueLength2: number = 0;
  showInputLength = (value: string) => {
    this.inputValueLength2 = value.length;
  }

  isChecked:boolean = false;
  toggle = (e: any) => {
    this.isChecked = !this.isChecked;
  }

  yourName: string = "";
  inputYourName(e: any) {
    this.yourName = e.target.value;
  }
  submit =() => {
    alert(`ようこそ${this.yourName}さん`)
  }

  maxAllowedFiles: number = 1;
  maxFileSize = 1024 * 1024 * 20; // 20MB
  UploadFileAsync = async (files: FileList | null) => {
    if (files === null) return;

    const fileContents = new FormData();
    for (let i = 0; i < files.length; i++) {
      try {
        console.log(files[i].name);
        fileContents.append("postalCsv", files[i], files[i].name)
      } catch (error) {
        console.log(error);
      }
    }
  }

}
