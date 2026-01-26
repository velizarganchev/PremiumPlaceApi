import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, signal } from '@angular/core';

import { RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PlacesService } from '../../../core/places/places.service';


type ReviewSummary = {
  avg: number;
  count: number;
};

@Component({
  selector: 'app-place-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatDividerModule,
  ],
  templateUrl: './place-details.component.html',
  styleUrl: './place-details.component.scss'
})
export class PlaceDetailsComponent {
  private placesService = inject(PlacesService);

  // route param
  id = input.required<string>();
  placeId = computed(() => Number(this.id()));

  ngOnInit() {
    this.placesService.byId(this.placeId()).subscribe();
  }

  place = this.placesService.place;
  loading = this.placesService.loadingPlace;
  // временно (после ще го вържеш с real reviews)
  review = signal<ReviewSummary>({ avg: 4.8, count: 2 });

  stars = computed(() => {
    const avg = this.review().avg;
    const full = Math.floor(avg);
    const half = avg - full >= 0.5;
    const empty = 5 - full - (half ? 1 : 0);
    return { full, half, empty };
  });

  // gallery helpers (ако после имаш масив от снимки)
  gallery = computed(() => {
    const p = this.place();
    const base = p?.imageUrl ? [p.imageUrl] : [];
    // placeholder допълнителни снимки (можеш да ги смениш)
    return [...base, ...base, ...base].slice(0, 3);
  });

  onShowAllPhotos() {
    // TODO: after: open dialog / full gallery page
    console.log('Show all photos');
  }
}
