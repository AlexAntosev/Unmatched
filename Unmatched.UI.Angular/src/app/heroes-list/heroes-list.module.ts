import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HeroesListComponent } from './heroes-list.component';
import { AppRoutingModule } from '../app-routing.module';
import { HeroesApi } from '../api/heroes.api';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';

@NgModule({
  declarations: [HeroesListComponent],
  imports: [BrowserModule, AppRoutingModule, MatTableModule, MatSortModule],
  providers: [HeroesApi],
})
export class HeroesListModule {}
