import { AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { HeroesApi } from '../api/heroes.api';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { HeroStatisticsDto } from '../api/models/hero-statistics-dto';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';

@UntilDestroy()
@Component({
  selector: 'app-heroes',
  standalone: false,
  templateUrl: './heroes.component.html',
  styleUrl: './heroes.component.css',
})
export class HeroesComponent implements OnInit, AfterViewInit {
  public heroesSbj$: BehaviorSubject<HeroStatisticsDto[]> = new BehaviorSubject<HeroStatisticsDto[]>([]);
  public heroes$: Observable<HeroStatisticsDto[]> = this.heroesSbj$.asObservable();

  columns: string[] = [
    'place',
    'image',
    'name',
    'range',
    'points',
    'matches',
    'wins',
    'looses',
    'kd',
  ];

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<HeroStatisticsDto>;

  constructor(private heroesApi: HeroesApi) {}

  ngOnInit(): void {
    this.heroesApi
      .getHeroesStatistics()
      .pipe(
        tap((heroes: HeroStatisticsDto[]) => {
          this.sortHeroes(heroes);
          this.setHeroes(heroes);
        }),
        untilDestroyed(this)
      )
      .subscribe();
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => {
      const heroes = this.heroesSbj$.getValue();
      this.sortHeroes(heroes);
      this.setHeroes(heroes);
    });
  }

  setHeroes(heroes: HeroStatisticsDto[]) {
    heroes.map((hero) => {
      hero.place = heroes.indexOf(hero) + 1;
    });
    this.heroesSbj$.next(heroes);
  }

  sortHeroes(heroes: HeroStatisticsDto[]) {
    heroes = heroes.sort((a, b) => {
      const isAsc = this.sort.direction === 'asc';
      switch (this.sort.active) {
        case 'place':
          return this.compare(a.place, b.place, isAsc);
        case 'name':
          return this.compare(a.hero.name, b.hero.name, isAsc);
        case 'range':
          return this.compare(a.hero.isRanged, b.hero.isRanged, isAsc);
        case 'points':
          return this.compare(a.points, b.points, isAsc);
        case 'matches':
          return this.compare(a.totalMatches, b.totalMatches, isAsc);
        case 'wins':
          return this.compare(a.totalWins, b.totalWins, isAsc);
        case 'looses':
          return this.compare(a.totalLooses, b.totalLooses, isAsc);
        case 'kd':
          return this.compare(a.kd, b.kd, isAsc);
        default:
          return 0;
      }
    });
  }

  compare(
    a: number | string | boolean,
    b: number | string | boolean,
    isAsc: boolean
  ) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }
}
