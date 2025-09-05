import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from '../app-routing.module';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { GridComponent } from './grid/grid.component';

@NgModule({
  declarations: [GridComponent],
  imports: [BrowserModule, AppRoutingModule, MatTableModule, MatSortModule],
  exports: [GridComponent],
})
export class SharedModule {}
