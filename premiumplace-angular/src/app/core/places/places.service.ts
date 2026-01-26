import { inject, Injectable, signal, computed } from '@angular/core';
import { tap, map, finalize } from 'rxjs';
import { PlacesApi } from './places.api';
import { mapPlace, mapPlaceToCard } from './places.mapper';
import type { PlacePreview, PlaceDto } from './places.models';

@Injectable({ providedIn: 'root' })
export class PlacesService {
    private api = inject(PlacesApi);

    private _places = signal<PlacePreview[]>([]);
    private _loadingList = signal(false);

    private _place = signal<PlaceDto | null>(null);
    private _loadingPlace = signal(false);

    places = this._places.asReadonly();
    loadingList = this._loadingList.asReadonly();

    place = this._place.asReadonly();
    loadingPlace = this._loadingPlace.asReadonly();

    byId(id: number) {
        this._loadingPlace.set(true);
        return this.api.getById(id).pipe(
            tap(dto => this._place.set(dto)),
            finalize(() => this._loadingPlace.set(false))
        );
    }

    loadAll() {
        this._loadingList.set(true);

        return this.api.list().pipe(
            map(dtos => dtos.map(mapPlace)),
            tap({
                next: (places) => this._places.set(places),
                error: () => this._places.set([]),
            }),
            finalize(() => this._loadingList.set(false))
        );
    }

    cards = computed(() =>
        this.places().map(mapPlaceToCard)
    );

}
