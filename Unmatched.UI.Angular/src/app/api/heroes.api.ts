import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { HeroDto } from "./models/hero-dto";
import { Observable } from "rxjs";
import { HeroStatisticsDto } from "./models/hero-statistics-dto";

@Injectable()
export class HeroesApi {
  constructor(private http: HttpClient) {
  }

  getHeroes(): Observable<HeroDto[]> {
    return this.http.get<HeroDto[]>('https://localhost:7080/api/heroes');
  }

  getHeroesStatistics(): Observable<HeroStatisticsDto[]> {
    return this.http.get<HeroStatisticsDto[]>('https://localhost:7080/api/heroes/statistics');
  } 
}