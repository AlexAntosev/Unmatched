import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from '../app-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PlayersComponent } from './players.component';
import { PlayersGrid } from './players-grid';

@NgModule({
  declarations: [PlayersComponent],
  imports: [BrowserModule, AppRoutingModule, SharedModule],
  providers: [PlayersGrid],
})
export class PlayersModule {}
