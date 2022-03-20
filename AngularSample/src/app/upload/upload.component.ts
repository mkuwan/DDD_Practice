import { Component, OnInit } from '@angular/core';
import {HttpClient, HttpEvent, HttpEventType, HttpHeaders, HttpRequest} from "@angular/common/http";
import {catchError, last, map, Observable, of, tap} from "rxjs";
import {Hero} from "../hero";


interface UploadResult {
  Uploaded: boolean;
  FileName?: string;
  StoredFileName?: string;
  ErrorCode: number;
}

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  maxAllowedFiles: number = 1;
  uploadResults: UploadResult[] = [];
  uploadFile: File | any;
  fileName: string = "";
  progressMessage: string = "";

  constructor(private http: HttpClient) { }

  private uploadUrl = "api/v1/upload";

  ngOnInit(): void {
  }

  OnInputFileChangeAsync = async (files: FileList | null) => {
    const maxfilesize = 1024 * 1024 * 20; // 20MB
    let upload = false;
    let errorMessage = "";

    if(files){
      this.fileName = files[0].name;
      this.uploadFile = files[0];
    }

  }

  OnUploadFilesAsync = async() => {

    /**
     * https://angular.jp/guide/http
     * プログレスイベントを有効にしてリクエストを行うには、
     * reportProgressオプションをtrueに設定してHttpRequestのインスタンスを作成し、
     * プログレスイベントを追跡できるようにします
     */

    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'multipart/form-data' }) };
    const formData = new FormData();

    formData.append("\"files\"", this.uploadFile, this.uploadFile.name);

    this.http.post(`${this.uploadUrl}/address`, formData)
      .subscribe(
        data => console.log(data),
        error => console.log(error)
        );
    //const re = this.http.get(this.uploadUrl + "/csv");
    //re.subscribe(),
    //  (err: any) => { console.error(err) },
    //  () => { };
  }

  private getEventMessage(event:HttpEvent<any>, file: File) {
    switch (event.type) {
      case HttpEventType.Sent:
        return `Uploading file "${file.name}" of size ${file.size}.`;

      case HttpEventType.UploadProgress:
        // Compute and show the % done:
        const percentDone = Math.round(100 * event.loaded / (event.total ?? 0));
        return `File "${file.name}" is ${percentDone}% uploaded.`;

      case HttpEventType.Response:
        return `File "${file.name}" was completely uploaded!`;

      default:
        return `File "${file.name}" surprising upload event: ${event.type}.`;
    }
  }

  private showProgress(message: any) {
    this.progressMessage = message;
  }

  private log = (message: string) => {
    console.log(`${message}`);
  }

  /**
   * 失敗したHttp操作を処理します。
   * アプリを持続させます。
   * @param operation? - 失敗した操作の名前
   * @param result?? - observableな結果として返す任意の値
   */
  private handleError<T>(operation?: File, result?: T) {
    return (error: any): Observable<T> => {

      // TODO: リモート上のロギング基盤にエラーを送信する
      console.error(error); // かわりにconsoleに出力

      // TODO: ユーザーへの開示のためにエラーの変換処理を改善する
      this.log(`${operation} failed: ${error.message}`);

      // 空の結果を返して、アプリを持続可能にする
      return of(result as T);
    };
  }
}

