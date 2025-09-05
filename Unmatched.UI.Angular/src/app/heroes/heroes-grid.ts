import { Injectable } from '@angular/core';
import { HeroStatisticsDto } from '../backend-api/models/hero-statistics-dto';
import { GridColumn } from '../shared/grid/grid-column';
import { GridColumnType } from '../shared/grid/grid-column-type';

@Injectable()
export class HeroesGrid {
  getColumns(): GridColumn<HeroStatisticsDto>[] {
    return [
      {
        id: 'place',
        header: 'Place',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.place ? item.place : 0;
        },
      },
      {
        id: 'image',
        header: 'Image',
        type: GridColumnType.Image,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.hero?.imageUrl ? item.hero.imageUrl.toString() : '';
        },
      },
      {
        id: 'name',
        header: 'Name',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.hero?.name ? item.hero.name : 'Unknown';
        },
      },
      {
        id: 'range',
        header: 'Range',
        type: GridColumnType.Image,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.hero?.meleeRangeImageUrl
            ? item.hero.meleeRangeImageUrl.toString()
            : '';
        },
      },
      {
        id: 'points',
        header: 'Points',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.points ? item.points : 0;
        },
      },
      {
        id: 'matches',
        header: 'Matches',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.totalMatches ? item.totalMatches : 0;
        },
      },
      {
        id: 'wins',
        header: 'Wins',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.totalWins ? item.totalWins : 0;
        },
      },
      {
        id: 'looses',
        header: 'Looses',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.totalLooses ? item.totalLooses : 0;
        },
      },
      {
        id: 'kd',
        header: 'K/D',
        type: GridColumnType.Text,
        valueGetter: (item: HeroStatisticsDto) => {
          return item.kd ? item.kd : 0;
        },
      },
    ];
  }
}
