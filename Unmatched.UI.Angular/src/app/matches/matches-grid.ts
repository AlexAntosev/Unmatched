import { Injectable } from '@angular/core';
import { GridColumn } from '../shared/grid/grid-column';
import { GridColumnType } from '../shared/grid/grid-column-type';
import { MatchLogDto } from '../backend-api/models/match-log-dto';

@Injectable()
export class MatchesGrid {
  getColumns(): GridColumn<MatchLogDto>[] {
    return [
      {
        id: 'date',
        header: 'Date',
        type: GridColumnType.Date,
        valueGetter: (item: MatchLogDto) => {
          return item.date ? new Date(item.date) : new Date();
        },
      },
      {
        id: 'tournament',
        header: 'Tournament',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          return item.tournamentName;
        },
      },
      {
        id: 'map',
        header: 'Map',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          return item.mapName;
        },
      },
      {
        id: 'fighterPlayer',
        header: 'Player',
        type: GridColumnType.Image,
        valueGetter: (item: MatchLogDto) => {
          const fighter = this.getFighter(item);
          return fighter ? fighter.playerImageUrl : '';
        },
      },
      {
        id: 'fighterHero',
        header: 'Hero',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const fighter = this.getFighter(item);
          return fighter?.hero ? fighter.hero.name : '';
        },
      },
      {
        id: 'fighterHpLeft',
        header: 'Hero Hp (Sidekick)',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const fighter = this.getFighter(item);
          return fighter
            ? `${fighter.hpLeft} ${
                fighter.sidekickHpLeft
                  ? ' (' + fighter.sidekickHpLeft + ')'
                  : ''
              }`
            : 0;
        },
      },
      {
        id: 'fighterCardsLeft',
        header: 'Hero Cards',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const fighter = this.getFighter(item);
          return fighter ? fighter.cardsLeft : 0;
        },
      },
      {
        id: 'fighterPoints',
        header: 'Points',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const fighter = this.getFighter(item);
          return fighter ? fighter.matchPoints : 0;
        },
      },
      {
        id: 'opponentPlayer',
        header: 'Player',
        type: GridColumnType.Image,
        valueGetter: (item: MatchLogDto) => {
          const opponent = this.getOpponent(item);
          return opponent ? opponent.playerImageUrl : '';
        },
      },
      {
        id: 'opponentHero',
        header: 'Hero',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const opponent = this.getOpponent(item);
          return opponent?.hero ? opponent.hero.name : '';
        },
      },
      {
        id: 'opponentHpLeft',
        header: 'Hero Hp (Sidekick)',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const opponent = this.getOpponent(item);
          return opponent
            ? `${opponent.hpLeft} ${
                opponent.sidekickHpLeft
                  ? ' (' + opponent.sidekickHpLeft + ')'
                  : ''
              }`
            : 0;
        },
      },
      {
        id: 'opponentCardsLeft',
        header: 'Hero Cards',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const opponent = this.getOpponent(item);
          return opponent ? opponent.cardsLeft : 0;
        },
      },
      {
        id: 'opponentPoints',
        header: 'Points',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          const opponent = this.getOpponent(item);
          return opponent ? opponent.matchPoints : 0;
        },
      },
      {
        id: 'comment',
        header: 'Comment',
        type: GridColumnType.Text,
        valueGetter: (item: MatchLogDto) => {
          return item.comment;
        },
      },
    ];
  }

  getFighter(item: MatchLogDto) {
    return item.fighters ? item.fighters[0] : null;
  }

  getOpponent(item: MatchLogDto) {
    return item.fighters ? item.fighters[1] : null;
  }
}
