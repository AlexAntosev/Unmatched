import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideHttpClient } from '@angular/common/http';
import { HeroesListModule } from './heroes-list/heroes-list.module';
import { LayoutModule } from './layout/layout.module';
import { HeroDetailsComponent } from './hero-details/hero-details.component';
import { MatchModule } from './match/match.module';

@NgModule({
  declarations: [
    AppComponent,
    HeroDetailsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HeroesListModule,
    LayoutModule,
    MatchModule,
  ],
  providers: [
    provideHttpClient(),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
