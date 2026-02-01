import { Component, computed, HostListener, input, output, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-place-gallery',
  standalone: true,
  imports: [MatIconModule, MatButtonModule],
  templateUrl: './place-gallery.component.html',
  styleUrl: './place-gallery.component.scss'
})
export class PlaceGalleryComponent {
  images = input.required<string[]>();
  startIndex = input<number>(0);

  closed = output<void>();
  currentIndex = signal(0);

  ngOnInit() {
    this.currentIndex.set(this.startIndex());
  }

  currentImage = computed(() => this.images()[this.currentIndex()]);

  prev() {
    const len = this.images().length;
    this.currentIndex.update(i => (i - 1 + len) % len);
  }

  next() {
    const len = this.images().length;
    this.currentIndex.update(i => (i + 1) % len);
  }

  close() {
    this.closed.emit();
  }

  onOverlayClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('gallery-overlay')) {
      this.close();
    }
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') {
      this.close();
    } else if (event.key === 'ArrowLeft') {
      this.prev();
    } else if (event.key === 'ArrowRight') {
      this.next();
    }
  }
}   