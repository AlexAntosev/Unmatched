import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from '../app-routing.module';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { GridComponent } from './grid/grid.component';
import { RateComponent } from './rate/rate.component';

@NgModule({
  declarations: [GridComponent, RateComponent],
  imports: [BrowserModule, AppRoutingModule, MatTableModule, MatSortModule],
  exports: [GridComponent, RateComponent],
})
export class SharedModule {}
