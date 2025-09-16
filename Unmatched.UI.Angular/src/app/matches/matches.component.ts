import { Component, DestroyRef, inject, Input, OnInit } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { GridColumn } from '../shared/grid/grid-column';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatchesService } from '../backend-api/services/matches.service';
import { MatchesGrid } from './matches-grid';
import { UiMatchLogDto } from '../backend-api/models/ui-match-log-dto';

@Component({
  selector: 'matches',
  standalone: false,
  templateUrl: './matches.component.html',
})
export class MatchesComponent implements OnInit {
  @Input() public matchesSbj$ = new BehaviorSubject<UiMatchLogDto[]>([]);
  public columns: GridColumn<UiMatchLogDto>[] = [];

  constructor(
    private readonly router: Router,
    private readonly matchesGrid: MatchesGrid
  ) {}

  ngOnInit(): void {
    this.columns = this.matchesGrid.getColumns();
  }

  onRowClicked(item: UiMatchLogDto) {
    this.router.navigate(['/match', item.matchId]);
  }
}
