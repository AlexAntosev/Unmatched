import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroesComponent } from './heroes/heroes.component';
import { HeroComponent } from './heroes/hero/hero.component';
import { PlayersComponent } from './players/players.component';
import { MatchComponent } from './matches/match/match.component';

const routes: Routes = [
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
