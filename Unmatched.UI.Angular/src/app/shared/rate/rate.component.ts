import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'rate',
  standalone: false,
  templateUrl: './rate.component.html',
})
export class RateComponent {
  @Input() rating = 0;
  @Input() max = 5;

  @Output() ratingChanged = new EventEmitter<number>();

  updateRating(star: number) {
    this.rating = star;
    this.ratingChanged.emit(this.rating);
  }

  createRange(value: number) {
    return new Array(value).fill(0).map((n, index) => index + 1);
  }
}
