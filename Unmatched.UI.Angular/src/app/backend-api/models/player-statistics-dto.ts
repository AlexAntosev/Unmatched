/* tslint:disable */
/* eslint-disable */
import { PlayerDto } from './player-dto';
export interface PlayerStatisticsDto {
  kd?: number;
  lastMatchPoints?: number;
  place?: number;
  player?: PlayerDto;
  playerId?: string;
  totalLooses?: number;
  totalMatches?: number;
  totalWins?: number;
}
