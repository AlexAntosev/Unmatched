/* tslint:disable */
/* eslint-disable */
import { HeroDto } from './hero-dto';
import { TitleDto } from './title-dto';
export interface HeroStatisticsDto {
  hero?: HeroDto;
  heroId?: string;
  kd?: number;
  lastMatchPoints?: number;
  place?: number;
  points?: number;
  titles?: null | Array<TitleDto>;
  totalLooses?: number;
  totalMatches?: number;
  totalWins?: number;
}
