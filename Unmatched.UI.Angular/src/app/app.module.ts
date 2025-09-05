import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideHttpClient } from '@angular/common/http';
import { HeroesModule } from './heroes/heroes.module';
import { LayoutModule } from './shared/layout/layout.module';
import { MatchesModule } from './matches/matches.module';
import { PlayersModule } from './players/players.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HeroesModule,
    PlayersModule,
    LayoutModule,
    MatchesModule,
  ],
  providers: [
    provideHttpClient(),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
