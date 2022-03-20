import { Component, OnInit } from '@angular/core';
import { Hero} from "../hero";
import { HEROES } from "../mock-heroes";
import { HeroService } from "../hero.service";
import { MessageService } from "../message.service";

@Component({
  selector: 'app-heroes',
  templateUrl: './heroes.component.html',
  styleUrls: ['./heroes.component.css']
})
export class HeroesComponent implements OnInit {

  heroes: Hero[] = [];

  constructor(private  heroService: HeroService) { }

  /**
   * 初期化ロジックを書く場所
   */
  ngOnInit(): void {
    this.getHeroes();
  }

  getHeroes = () => {
    this.heroService.getHeroes()
      .subscribe(heroes => this.heroes = heroes);
  }

  add(name: string) {
    name = name.trim();
    if(!name) return;

    this.heroService.addHero({name} as Hero)
      .subscribe(hero => {
        this.heroes.push(hero);
      })
  }

  delete(hero: Hero) {
    this.heroes = this.heroes.filter(x => x !== hero);
    this.heroService.deleteHero(hero.id).subscribe();
  }
}
