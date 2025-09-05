import { Component, DestroyRef, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HeroesService } from '../../backend-api/services/heroes.service';
import { tap } from 'rxjs';
import { HeroStatisticsDto } from '../../backend-api/models/hero-statistics-dto';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'hero',
  standalone: false,
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.css',
})
export class HeroComponent {
  public heroStatistics: HeroStatisticsDto | null = null;

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
    private route: ActivatedRoute,
    private heroesService: HeroesService
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
  }
}
