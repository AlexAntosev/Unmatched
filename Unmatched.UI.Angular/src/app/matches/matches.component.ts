import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { GridColumn } from '../shared/grid/grid-column';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatchLogDto } from '../backend-api/models/match-log-dto';
import { MatchesService } from '../backend-api/services/matches.service';
import { MatchesGrid } from './matches-grid';

@Component({
  selector: 'matches',
  standalone: false,
  templateUrl: './matches.component.html',
})
export class MatchesComponent implements OnInit {
  public matchesSbj$ = new BehaviorSubject<MatchLogDto[]>([]);
  public columns: GridColumn<MatchLogDto>[] = [];

  private destroyRef = inject(DestroyRef);

  constructor(
    private readonly router: Router,
    private readonly matchesService: MatchesService,
    private readonly matchesGrid: MatchesGrid
  ) {}

  ngOnInit(): void {
    this.columns = this.matchesGrid.getColumns();
    this.matchesService
      .apiMatchesGet()
      .pipe(
        tap((matches: MatchLogDto[]) => {
          this.matchesSbj$.next(matches);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  onRowClicked(item: MatchLogDto) {
    this.router.navigate(['/match', item.matchId]);
  }
}
