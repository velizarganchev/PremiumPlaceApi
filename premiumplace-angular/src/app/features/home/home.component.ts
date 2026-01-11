import { CommonModule } from '@angular/common';
import { Component, computed, signal } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';

import { HeroComponent } from "./hero/hero.component";
import { CardItem } from '../../shared/ui/cards-grid/cards-grid.model';
import { CardsGridComponent } from "../../shared/ui/cards-grid/cards-grid.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatButtonModule, HeroComponent, CardsGridComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  cards: CardItem[] = [
    {
      id: '1',
      title: 'New Luxury Penthouse',
      subtitle: 'Anaheim, California',
      priceText: '$122 / night',
      meta: '★★★★★ (2 reviews)',
      imageUrl: 'https://images.unsplash.com/photo-1560448204-603b3fc33ddc?auto=format&fit=crop&w=1600&q=80',
    },
    {
      id: '2',
      title: 'BBQ Patio • Smart TV • 20min Beach',
      subtitle: 'Fountain Valley, California',
      priceText: '$130 / night',
      meta: '★★★★★ (1 review)',
      imageUrl: 'https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=1600&q=80',
    },
  ];
}
