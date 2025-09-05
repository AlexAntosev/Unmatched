import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HeroesComponent } from './heroes.component';
import { AppRoutingModule } from '../app-routing.module';
import { SharedModule } from '../shared/shared.module';
import { HeroComponent } from './hero/hero.component';
import { HeroesGrid } from './heroes-grid';

@NgModule({
  declarations: [HeroesComponent, HeroComponent],
  imports: [BrowserModule, AppRoutingModule, SharedModule],
  providers: [HeroesGrid],
})
export class HeroesModule {}
