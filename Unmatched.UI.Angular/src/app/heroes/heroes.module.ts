import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HeroesComponent } from './heroes.component';
import { AppRoutingModule } from '../app-routing.module';
import { SharedModule } from '../shared/shared.module';
import { HeroComponent } from './hero/hero.component';
import { HeroesGrid } from './heroes-grid';
import { MatchesModule } from '../matches/matches.module';

@NgModule({
  declarations: [HeroesComponent, HeroComponent],
  imports: [BrowserModule, AppRoutingModule, SharedModule, MatchesModule],
  providers: [HeroesGrid],
})
export class HeroesModule {}
