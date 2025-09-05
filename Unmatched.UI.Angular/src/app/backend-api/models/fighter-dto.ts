/* tslint:disable */
/* eslint-disable */
import { HeroDto } from './hero-dto';
import { PlayerDto } from './player-dto';
export interface FighterDto {
  actionsMade?: null | number;
  cardsLeft?: null | number;
  hero?: HeroDto;
  heroId?: string;
  heroImageUrl?: null | string;
  hpLeft?: null | number;
  id?: string;
  isWinner?: boolean;
  itemsUsed?: null | number;
  matchId?: string;
  matchPoints?: null | number;
  player?: PlayerDto;
  playerId?: string;
  playerImageUrl?: null | string;
  sidekickHpLeft?: null | number;
  sidekickName?: null | string;
  timeSpentInSeconds?: null | number;
  turn?: null | number;
}
