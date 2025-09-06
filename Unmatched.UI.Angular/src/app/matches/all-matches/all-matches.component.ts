import { Component, DestroyRef, inject, Input, OnInit } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatchLogDto } from '../../backend-api/models/match-log-dto';
import { MatchesService } from '../../backend-api/services/matches.service';

@Component({
  selector: 'all-matches',
  standalone: false,
  templateUrl: './all-matches.component.html',
})
export class AllMatchesComponent implements OnInit {
  public matchesSbj$ = new BehaviorSubject<MatchLogDto[]>([]);

  private destroyRef = inject(DestroyRef);

  constructor(private readonly matchesService: MatchesService) {}

  ngOnInit(): void {
    this.matchesService
      .apiMatchesGet()
      .pipe(
        tap((matches: MatchLogDto[]) => {
          this.matchesSbj$.next(matches);
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }
}
