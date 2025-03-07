import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HeroesComponent } from './heroes.component';
import { AppRoutingModule } from '../app-routing.module';
import { HeroesApi } from '../api/heroes.api';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';

@NgModule({
  declarations: [HeroesComponent],
  imports: [BrowserModule, AppRoutingModule, MatTableModule, MatSortModule],
  providers: [HeroesApi],
})
export class HeroesModule {}
