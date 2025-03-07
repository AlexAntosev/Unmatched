import { HeroDto } from "./hero-dto";
import { TitleDto } from "./title-dto";

export class HeroStatisticsDto {
    hero: HeroDto;
    heroId: string;
    lastMatchPoints: number;
    points: number;
    place: number;
    totalLooses: number;
    totalMatches: number;
    totalWins: number;
    titles: TitleDto[];

    constructor(data: HeroStatisticsDto) {
        this.hero = data.hero;
        this.heroId = data.heroId;
        this.lastMatchPoints = data.lastMatchPoints;
        this.points = data.points;
        this.place = data.place;
        this.totalLooses = data.totalLooses;
        this.totalMatches = data.totalMatches;
        this.totalWins = data.totalWins;
        this.titles = data.titles;
    }

    get kd(): number {
        return this.totalMatches > 0
            ? Math.round((this.totalWins / this.totalMatches) * 100) / 100
            : 0;
    }
}