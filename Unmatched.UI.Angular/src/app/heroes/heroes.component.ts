import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { HeroesService } from '../backend-api/services/heroes.service';
import { HeroStatisticsDto } from '../backend-api/models/hero-statistics-dto';
import { GridColumn } from '../shared/grid/grid-column';
import { HeroesGrid } from './heroes-grid';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'heroes',
  standalone: false,
  templateUrl: './heroes.component.html',
})
export class HeroesComponent implements OnInit {
  public heroesSbj$ = new BehaviorSubject<HeroStatisticsDto[]>([]);
  public columns: GridColumn<HeroStatisticsDto>[] = [];

  private destroyRef = inject(DestroyRef);

  constructor(
    private readonly router: Router,
    private readonly heroesService: HeroesService,
    private readonly heroesGrid: HeroesGrid
  ) {}

  ngOnInit(): void {
    this.columns = this.heroesGrid.getColumns();
    this.heroesService
      .apiHeroesStatisticsGet()
      .pipe(
        tap((heroes: HeroStatisticsDto[]) => {
          this.heroesSbj$.next(heroes);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  onRowClicked(item: HeroStatisticsDto) {
    this.router.navigate(['/heroes', item.heroId]);
  }
}
