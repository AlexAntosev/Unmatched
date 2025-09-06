import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroesComponent } from './heroes/heroes.component';
import { HeroComponent } from './heroes/hero/hero.component';
import { PlayersComponent } from './players/players.component';
import { MatchComponent } from './matches/match/match.component';
import { MatchesComponent } from './matches/matches.component';
import { AllMatchesComponent } from './matches/all-matches/all-matches.component';

const routes: Routes = [
  {
    path: 'matches',
    component: AllMatchesComponent,
  },
  {
    path: 'match',
    component: MatchComponent,
  },
  {
    path: 'heroes',
    component: HeroesComponent,
  },
  {
    path: 'heroes/:id',
    component: HeroComponent,
  },
  {
    path: 'players',
    component: PlayersComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
