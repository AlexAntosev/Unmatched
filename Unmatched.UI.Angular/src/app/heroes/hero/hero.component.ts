import { Component, DestroyRef, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HeroesService } from '../../backend-api/services/heroes.service';
import { BehaviorSubject, tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatchesService } from '../../backend-api/services/matches.service';
import { UiHeroStatisticsDto } from '../../backend-api/models/ui-hero-statistics-dto';
import { UiMatchLogDto } from '../../backend-api/models/ui-match-log-dto';

@Component({
  selector: 'hero',
  standalone: false,
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.css',
})
export class HeroComponent {
  public heroStatistics: UiHeroStatisticsDto | null = null;
  public matchesSbj$ = new BehaviorSubject<UiMatchLogDto[]>([]);

  public get sidekick() {
    return this.heroStatistics?.sidekicks
      ? this.heroStatistics.sidekicks[0]
      : null;
  }

  public get playstyle() {
    return this.heroStatistics?.playStyle
      ? this.heroStatistics.playStyle
      : null;
  }

  public get titles() {
    return this.heroStatistics?.titles
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
        tap((matches: UiMatchLogDto[]) => {
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
