import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MatchComponent } from './match/match.component';
import { HeroesComponent } from './heroes/heroes.component';
import { HeroComponent } from './hero/hero.component';

const routes: Routes = [{
  path: 'match',
  component: MatchComponent
}, 
{
  path: 'heroes',
  component: HeroesComponent
}, 
{
  path: 'heroes/:id',
  component: HeroComponent
}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
