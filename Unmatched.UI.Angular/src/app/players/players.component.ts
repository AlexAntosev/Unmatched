import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { GridColumn } from '../shared/grid/grid-column';
import { PlayerStatisticsDto } from '../backend-api/models/player-statistics-dto';
import { PlayersService } from '../backend-api/services/players.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PlayersGrid } from './players-grid';

@Component({
  selector: 'app-players',
  standalone: false,
  templateUrl: './players.component.html',
})
export class PlayersComponent implements OnInit {
  public playersSbj$ = new BehaviorSubject<PlayerStatisticsDto[]>([]);
  public columns: GridColumn<PlayerStatisticsDto>[] = [];

  private destroyRef = inject(DestroyRef);

  constructor(
    private readonly router: Router,
    private readonly playersService: PlayersService,
    private readonly playersGrid: PlayersGrid
  ) {}

  ngOnInit(): void {
    this.columns = this.playersGrid.getColumns();
    this.playersService
      .apiPlayersStatisticsGet()
      .pipe(
        tap((players: PlayerStatisticsDto[]) => {
          this.playersSbj$.next(players);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  onRowClicked(item: PlayerStatisticsDto) {
    this.router.navigate(['/player', item.playerId]);
  }
}
