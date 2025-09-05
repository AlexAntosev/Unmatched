/* tslint:disable */
/* eslint-disable */
import { PlayStyleDto } from './play-style-dto';
import { SidekickDto } from './sidekick-dto';
import { TitleDto } from './title-dto';
export interface HeroDto {
  color?: null | string;
  deckSize?: number;
  hp?: number;
  id?: string;
  imageUrl?: null | string;
  isRanged?: boolean;
  meleeRangeImageUrl?: null | string;
  name?: null | string;
  playStyle?: PlayStyleDto;
  sidekicks?: null | Array<SidekickDto>;
  titles?: null | Array<TitleDto>;
}
