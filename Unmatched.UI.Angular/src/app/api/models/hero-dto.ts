import { PlayStyleDto } from './playstyle-dto';
import { SidekickDto } from './sidekick-dto';
import { TitleDto } from './title-dto';

export class HeroDto {
  public deckSize: number;
  public hp: number;
  public id: string;
  public isRanged: boolean;
  public name: string;
  public sidekicks: SidekickDto[];
  public color: string;
  public titles: TitleDto[];
  public playStyle: PlayStyleDto;

  constructor() {
    this.deckSize = 0;
    this.hp = 0;
    this.id = '';
    this.isRanged = false;
    this.name = '';
    this.sidekicks = [];
    this.color = '';
    this.titles = [];
    this.playStyle = {} as PlayStyleDto;
  }

  public get ImageUrl(): string {
    return `/${this.name}.png`;
  }

  public get MeleeRangeImageUrl(): string {
    return `/${this.isRanged ? 'Ranged' : 'Melee'}.png`;
  }
}
