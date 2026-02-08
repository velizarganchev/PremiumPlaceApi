import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  input,
  output,
  signal,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { DateRange, MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-place-booking-widget',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatNativeDateModule,
  ],
  templateUrl: './place-booking-widget.component.html',
  styleUrls: ['./place-booking-widget.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlaceBookingWidgetComponent {
  // --- Inputs ---
  placeId = input.required<number>();
  rate = input.required<number>();
  reviewAvg = input.required<number>();
  reviewCount = input.required<number>();
  currencySymbol = input<string>('€');

  // --- Outputs (signal-based) ---
  readonly dateRangeChange = output<{ start: Date | null; end: Date | null }>();
  readonly bookClicked = output<{ placeId: number; start: Date; end: Date }>();

  // --- Reactive form ---
  readonly form = new FormGroup({
    start: new FormControl<Date | null>(null, Validators.required),
    end: new FormControl<Date | null>(null, Validators.required),
  });

  // --- Signals ---
  private readonly formValue = toSignal(this.form.valueChanges);

  readonly range = computed(() => {
    const v = this.formValue();
    return {
      start: v?.start ?? null,
      end: v?.end ?? null,
    };
  });

  readonly errorText = signal<string | null>(null);

  // --- Computed ---

  /** DateRange fed to the inline mat-calendar. */
  readonly selectedDateRange = computed(
    () => new DateRange<Date>(this.range().start, this.range().end),
  );

  /** Number of nights between start and end (0 when invalid). */
  readonly nights = computed(() => {
    const { start, end } = this.range();
    if (!start) return 0;

    // single-day selection = 1 night
    if (end && end.getTime() === start.getTime()) {
      return 1;
    }

    if (!end) return 0;

    const ms = end.getTime() - start.getTime();
    const n = Math.floor(ms / (1000 * 60 * 60 * 24));
    return n > 0 ? n : 0;
  });

  /** Whether the Book button should be enabled. */
  readonly canBook = computed(() => this.nights() > 0);

  /** Total price for the selected stay. */
  readonly totalPrice = computed(() => this.nights() * this.rate());

  /** Earliest selectable date (today). */
  readonly minDate = new Date();

  // --- Private ---
  private _selectingEnd = false;

  constructor() {
    effect(() => {
      const v = this.formValue();
      if (v === undefined) return;

      const start = v.start ?? null;
      const end = v.end ?? null;

      this.dateRangeChange.emit({ start, end });
      this.errorText.set(null);

      // Keep calendar click state in sync with form values
      if (start && end) this._selectingEnd = false;
      else if (!start) this._selectingEnd = false;
    }, { allowSignalWrites: true });
  }

  // --- Handlers ---

  /**
   * Two-click calendar selection: first click sets start, second sets end.
   * If the second date is before the first, the pair is swapped automatically.
   */
  onCalendarDateClicked(date: Date | null): void {
    if (!date) return;

    const currentStart = this.form.controls.start.value;

    if (!this._selectingEnd || !currentStart) {
      this.form.controls.start.setValue(date);
      this.form.controls.end.setValue(null);
      this._selectingEnd = true;
      return;
    }

    if (date > currentStart) {
      this.form.controls.end.setValue(date);
    } else {
      // swap
      this.form.controls.end.setValue(currentStart);
      this.form.controls.start.setValue(date);
    }
    this._selectingEnd = false;
  }

  /** Emits bookClicked when the selected range is valid. */
  onBook(): void {
    if (!this.canBook()) return;

    const start = this.form.controls.start.value!;
    let end = this.form.controls.end.value!;

    if (start && end && start.getTime() === end.getTime()) {
      end = new Date(start);
      end.setDate(end.getDate() + 1);
    }

    if (!start || !end) return;

    // TODO: check auth before booking
    // TODO: integrate availability endpoint + disable blocked dates using dateFilter
    // TODO: handle API errors

    this.bookClicked.emit({ placeId: this.placeId(), start, end });
  }
}
