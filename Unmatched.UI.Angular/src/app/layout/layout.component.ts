import { Component, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  standalone: false,
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent {
  @ViewChild('sidenav') sidenav: MatSidenav = {} as MatSidenav;

  constructor(private readonly router: Router) {}

  menuClicked() {
    this.sidenav.opened ? this.sidenav.close() : this.sidenav.open();
  }

  heroesClicked() {
    this.router.navigate(['heroes']);
  }

  matchesClicked() {
    this.router.navigate(['match']);
  }
}
