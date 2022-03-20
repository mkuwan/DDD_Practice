import { Injectable } from '@angular/core';
import { Hero } from "./hero";
import { HEROES } from "./mock-heroes";
import {catchError, Observable, of, tap} from "rxjs";
import { MessageService } from "./message.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class HeroService {
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  }

  constructor(private messageService: MessageService,
              private http: HttpClient) { }

  private heroesUrl = "hero";

  getHeroes = (): Observable<Hero[]> => {
    // const heroes = of(HEROES);

    /**
     * HttpClient.get()はデフォルトではレスポンスの本文を型のないJSONで返します
     */
    return this.http.get<Hero[]>("api/hero")
      .pipe(
        tap(heroes => this.log("fetched heroes!")),
        catchError(this.handleError<Hero[]>('getHeroes', []))
      );
  }
  /**
   *  constructor(http: HttpClient) {
    const re = http.get<WeatherForecast[]>("weatherforecast");
    re.subscribe((x: any) => this.forecasts = x,
      (err: any) => {console.error(err)},
      () => {} );
   *
   * @param message
   */


  private log = (message: string) => {
    this.messageService.add(`HeroService: ${message}`);
  }

  getHero(id: number): Observable<Hero> {
    /**
     * ! をつけるとundefinedではないことが明示される
     * ? がundefinedがあるかも　という意味の反対ということかな
     */
    // const hero = HEROES.find(h => h.id === id)!;
    // this.messageService.add(`HeroService: fetched hero id=${id}`);
    // return of(hero);

    const url = `${this.heroesUrl}/${id}`;
    return this.http.get<Hero>(url)
      .pipe(
        tap(_ => this.log(`fetched hero id=${id}`)),
        catchError(this.handleError<Hero>(`getHero id=${id}`))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);

      this.log(`${operation} failed: ${error.message}`);

      return of(result as T);
    }
  }

  updateHero(hero: Hero): Observable<any> {
    return this.http.put(this.heroesUrl, hero, this.httpOptions)
      .pipe(
        tap(_ => this.log(`updated hero id=${hero.id}`)),
        catchError(this.handleError<any>('updateHero'))
      );
  }

  addHero(hero: Hero): Observable<Hero> {
    return this.http.post<Hero>(this.heroesUrl, hero, this.httpOptions)
      .pipe(
        tap((newHero: Hero) => this.log(`added new hero id=${newHero.id}`)),
        catchError(this.handleError<Hero>('addHero'))
      );
  }

  deleteHero(id: number): Observable<Hero> {
    const url = `${this.heroesUrl}/${id}`;

    return this.http.delete<Hero>(url, this.httpOptions)
      .pipe(
        tap(_ => this.log(`deleted hero id=${id}`)),
        catchError(this.handleError<Hero>('deleteHero'))
      );
  }

  searchHeroes = (term: string): Observable<Hero[]> => {
    if(!term.trim()) return of([]);

    return this.http.get<Hero[]>(`${this.heroesUrl}/?name=${term}`)
      .pipe(
        tap(_ => this.log(`found heroes matching ${term}`)),
        catchError(this.handleError<Hero[]>('searchHeroes'))
      );
  }
}
