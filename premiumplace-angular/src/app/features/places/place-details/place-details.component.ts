import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, signal } from '@angular/core';

import { RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PlacesService } from '../../../core/places/places.service';
import { PlaceGalleryComponent } from '../place-gallery/place-gallery.component';

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
    PlaceGalleryComponent,
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

  stars = computed(() => {
    const avg = this.place()!.reviewSummary.avg;
    const full = Math.floor(avg);
    const half = avg - full >= 0.5;
    const empty = 5 - full - (half ? 1 : 0);
    return { full, half, empty };
  });

  gallery = computed(() => {
    const p = this.place();
    const base = p?.imageUrl ? [p.imageUrl] : [];
    return [...base, ...base, ...base].slice(0, 3);
  });

  showGallery = signal(false);

  onShowAllPhotos() {
    this.showGallery.set(true);
  }

  onCloseGallery() {
    this.showGallery.set(false);
  }
}
