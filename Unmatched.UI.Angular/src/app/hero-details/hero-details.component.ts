import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hero-details',
  standalone: false,
  templateUrl: './hero-details.component.html',
  styleUrl: './hero-details.component.css',
})
export class HeroDetailsComponent {
  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      console.log(params.get('id'));
    });
  }
}
