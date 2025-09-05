/* tslint:disable */
/* eslint-disable */
import { HeroDto } from './hero-dto';
export interface SidekickDto {
  count?: number;
  hero?: HeroDto;
  heroId?: string;
  hp?: number;
  id?: string;
  isRanged?: boolean;
  meleeRangeImageUrl?: null | string;
  name?: null | string;
}
