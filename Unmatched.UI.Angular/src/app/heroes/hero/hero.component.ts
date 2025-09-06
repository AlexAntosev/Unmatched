import { Component, DestroyRef, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HeroesService } from '../../backend-api/services/heroes.service';
import { BehaviorSubject, tap } from 'rxjs';
import { HeroStatisticsDto } from '../../backend-api/models/hero-statistics-dto';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatchesService } from '../../backend-api/services/matches.service';
import { MatchLogDto } from '../../backend-api/models/match-log-dto';

@Component({
  selector: 'hero',
  standalone: false,
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.css',
})
export class HeroComponent {
  public heroStatistics: HeroStatisticsDto | null = null;
  public matchesSbj$ = new BehaviorSubject<MatchLogDto[]>([]);

  public get hero() {
    return this.heroStatistics?.hero ? this.heroStatistics.hero : null;
  }

  public get sidekick() {
    return this.heroStatistics?.hero?.sidekicks
      ? this.heroStatistics.hero.sidekicks[0]
      : null;
  }

  public get playstyle() {
    return this.heroStatistics?.hero?.playStyle
      ? this.heroStatistics.hero.playStyle
      : null;
  }

  public get titles() {
    return this.heroStatistics?.hero?.titles
      ? this.heroStatistics.titles?.map((t) => t.name).join(', ')
      : null;
  }

  private heroId: string | null = null;

  private destroyRef = inject(DestroyRef);

  constructor(
    private readonly route: ActivatedRoute,
    private readonly heroesService: HeroesService,
    private readonly matchesService: MatchesService
  ) {}

  ngOnInit() {
    this.heroId = this.route.snapshot.params['id'];

    if (!this.heroId) {
      return;
    }

    this.heroesService
      .apiHeroesStatisticsHeroIdGet({ heroId: this.heroId })
      .pipe(
        tap((heroStatistics) => (this.heroStatistics = heroStatistics)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();

    this.matchesService
      .apiMatchesHeroIdGet({ heroId: this.heroId })
      .pipe(
        tap((matches: MatchLogDto[]) => {
          this.matchesSbj$.next(matches);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  playStyleAttackChanged(value: number) {
    if (this.heroId && this.playstyle) {
      this.playstyle.attack = value;
      this.heroesService
        .apiHeroesPlaystyleHeroIdPut({
          heroId: this.heroId,
          body: this.playstyle,
        })
        .subscribe();
    }
  }

  playStyleDefenceChanged(value: number) {
    if (this.heroId && this.playstyle) {
      this.playstyle.defence = value;
      this.heroesService
        .apiHeroesPlaystyleHeroIdPut({
          heroId: this.heroId,
          body: this.playstyle,
        })
        .subscribe();
    }
  }

  playStyleTrickeryChanged(value: number) {
    if (this.heroId && this.playstyle) {
      this.playstyle.trickery = value;
      this.heroesService
        .apiHeroesPlaystyleHeroIdPut({
          heroId: this.heroId,
          body: this.playstyle,
        })
        .subscribe();
    }
  }

  playStyleDifficultyChanged(value: number) {
    if (this.heroId && this.playstyle) {
      this.playstyle.difficulty = value;
      this.heroesService
        .apiHeroesPlaystyleHeroIdPut({
          heroId: this.heroId,
          body: this.playstyle,
        })
        .subscribe();
    }
  }
}
