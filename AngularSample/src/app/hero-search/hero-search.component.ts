import { Component, OnInit } from '@angular/core';
import {HeroService} from "../hero.service";
import {debounceTime, distinctUntilChanged, Observable, Subject, switchMap} from "rxjs";
import {Hero} from "../hero";

@Component({
  selector: 'app-hero-search',
  templateUrl: './hero-search.component.html',
  styleUrls: ['./hero-search.component.css']
})
export class HeroSearchComponent implements OnInit {

  heroes$!: Observable<Hero[]>;
  private searchTerms = new Subject<string>()

  constructor(private heroService: HeroService) { }

  ngOnInit(): void {
    this.heroes$ = this.searchTerms
      .pipe(
        // waiting 300ms
        debounceTime(300),

        // 直前の検索語と同じ場合は無視する
        distinctUntilChanged(),

        // 検索語が変わる度に、新しい検索observableにスイッチする
        switchMap((term:string) => this.heroService.searchHeroes(term)),
      );
  }

  search(term: string) {
    this.searchTerms.next(term);
  }


}
