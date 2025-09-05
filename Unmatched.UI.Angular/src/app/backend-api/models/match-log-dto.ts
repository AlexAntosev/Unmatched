/* tslint:disable */
/* eslint-disable */
import { FighterDto } from './fighter-dto';
export interface MatchLogDto {
  comment?: null | string;
  date?: string;
  epic?: null | number;
  fighters?: null | Array<FighterDto>;
  mapName?: null | string;
  matchId?: string;
  tournamentName?: null | string;
}
