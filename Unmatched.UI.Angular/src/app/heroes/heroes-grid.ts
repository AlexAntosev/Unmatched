import { Injectable } from '@angular/core';
import { GridColumn } from '../shared/grid/grid-column';
import { GridColumnType } from '../shared/grid/grid-column-type';
import { UiHeroStatisticsDto } from '../backend-api/models/ui-hero-statistics-dto';

@Injectable()
export class HeroesGrid {
  getColumns(): GridColumn<UiHeroStatisticsDto>[] {
    return [
      {
        id: 'place',
        header: 'Place',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.place ? item.place : 0;
        },
      },
      {
        id: 'image',
        header: 'Image',
        type: GridColumnType.Image,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.imageUrl ? item.imageUrl.toString() : '';
        },
      },
      {
        id: 'name',
        header: 'Name',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.name ? item.name : 'Unknown';
        },
      },
      {
        id: 'range',
        header: 'Range',
        type: GridColumnType.Image,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.meleeRangeImageUrl
            ? item.meleeRangeImageUrl.toString()
            : '';
        },
      },
      {
        id: 'points',
        header: 'Points',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.points ? item.points : 0;
        },
      },
      {
        id: 'matches',
        header: 'Matches',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.totalMatches ? item.totalMatches : 0;
        },
      },
      {
        id: 'wins',
        header: 'Wins',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.totalWins ? item.totalWins : 0;
        },
      },
      {
        id: 'looses',
        header: 'Looses',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.totalLooses ? item.totalLooses : 0;
        },
      },
      {
        id: 'kd',
        header: 'K/D',
        type: GridColumnType.Text,
        valueGetter: (item: UiHeroStatisticsDto) => {
          return item.kd ? item.kd : 0;
        },
      },
    ];
  }
}
