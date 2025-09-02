import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MatchComponent } from './match/match.component';
import { HeroesListComponent } from './heroes-list/heroes-list.component';
import { HeroDetailsComponent } from './hero-details/hero-details.component';

const routes: Routes = [{
  path: 'match',
  component: MatchComponent
}, 
{
  path: 'heroes',
  component: HeroesListComponent
}, 
{
  path: 'heroes/:id',
  component: HeroDetailsComponent
}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
