import { Injectable } from '@angular/core';
import { GridColumn } from '../shared/grid/grid-column';
import { GridColumnType } from '../shared/grid/grid-column-type';
import { PlayerStatisticsDto } from '../backend-api/models/player-statistics-dto';

@Injectable()
export class PlayersGrid {
  getColumns(): GridColumn<PlayerStatisticsDto>[] {
    return [
      {
        id: 'place',
        header: 'Place',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.place ? item.place : 0;
        },
      },
      {
        id: 'image',
        header: 'Image',
        type: GridColumnType.Image,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.imageUrl ? item.imageUrl.toString() : '';
        },
      },
      {
        id: 'name',
        header: 'Name',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.name ? item.name : 'Unknown';
        },
      },
      {
        id: 'matches',
        header: 'Matches',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.totalMatches ? item.totalMatches : 0;
        },
      },
      {
        id: 'wins',
        header: 'Wins',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.totalWins ? item.totalWins : 0;
        },
      },
      {
        id: 'looses',
        header: 'Looses',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.totalLooses ? item.totalLooses : 0;
        },
      },
      {
        id: 'kd',
        header: 'K/D',
        type: GridColumnType.Text,
        valueGetter: (item: PlayerStatisticsDto) => {
          return item.kd ? item.kd : 0;
        },
      },
    ];
  }
}
