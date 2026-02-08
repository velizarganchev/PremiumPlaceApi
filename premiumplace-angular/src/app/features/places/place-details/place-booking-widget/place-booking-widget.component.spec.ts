import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatNativeDateModule } from '@angular/material/core';

import { PlaceBookingWidgetComponent } from './place-booking-widget.component';

describe('PlaceBookingWidgetComponent', () => {
  let component: PlaceBookingWidgetComponent;
  let fixture: ComponentFixture<PlaceBookingWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        PlaceBookingWidgetComponent,
        NoopAnimationsModule,
        MatNativeDateModule,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PlaceBookingWidgetComponent);
    component = fixture.componentInstance;

    // Provide required signal inputs
    fixture.componentRef.setInput('placeId', 1);
    fixture.componentRef.setInput('rate', 99);
    fixture.componentRef.setInput('reviewAvg', 4.5);
    fixture.componentRef.setInput('reviewCount', 42);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render price and rating', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.textContent).toContain('€99');
    expect(el.textContent).toContain('/ night');
    expect(el.textContent).toContain('4.5');
    expect(el.textContent).toContain('(42)');
  });

  it('should disable Book when no dates', () => {
    const btn = fixture.nativeElement.querySelector(
      '.book-btn',
    ) as HTMLButtonElement;
    expect(btn.disabled).toBeTrue();
  });

  it('should enable Book when valid range selected', () => {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);

    component.form.controls.start.setValue(today);
    component.form.controls.end.setValue(tomorrow);
    fixture.detectChanges();

    const btn = fixture.nativeElement.querySelector(
      '.book-btn',
    ) as HTMLButtonElement;
    expect(btn.disabled).toBeFalse();
  });

  it('should compute nights correctly', () => {
    const start = new Date(2026, 5, 10);
    const end = new Date(2026, 5, 13);

    component.form.controls.start.setValue(start);
    component.form.controls.end.setValue(end);

    expect(component.nights()).toBe(3);
  });

  it('should emit dateRangeChange on selection', () => {
    spyOn(component.dateRangeChange, 'emit');

    const start = new Date(2026, 6, 1);
    component.form.controls.start.setValue(start);
    fixture.detectChanges(); // flush the effect

    expect(component.dateRangeChange.emit).toHaveBeenCalledWith({
      start,
      end: null,
    });
  });

  it('should emit bookClicked with correct payload', () => {
    spyOn(component.bookClicked, 'emit');

    const start = new Date(2026, 6, 1);
    const end = new Date(2026, 6, 5);

    component.form.controls.start.setValue(start);
    component.form.controls.end.setValue(end);
    fixture.detectChanges();

    component.onBook();

    expect(component.bookClicked.emit).toHaveBeenCalledWith({
      placeId: 1,
      start,
      end,
    });
  });

  it('should not emit bookClicked when range is invalid', () => {
    spyOn(component.bookClicked, 'emit');

    component.onBook();

    expect(component.bookClicked.emit).not.toHaveBeenCalled();
  });

  it('should handle calendar two-click selection', () => {
    const dateA = new Date(2026, 4, 10);
    const dateB = new Date(2026, 4, 15);

    component.onCalendarDateClicked(dateA);
    expect(component.form.controls.start.value).toEqual(dateA);
    expect(component.form.controls.end.value).toBeNull();

    component.onCalendarDateClicked(dateB);
    expect(component.form.controls.start.value).toEqual(dateA);
    expect(component.form.controls.end.value).toEqual(dateB);
    expect(component.nights()).toBe(5);
  });

  it('should swap dates when second click is before first', () => {
    const earlier = new Date(2026, 4, 5);
    const later = new Date(2026, 4, 10);

    component.onCalendarDateClicked(later);
    component.onCalendarDateClicked(earlier);

    expect(component.form.controls.start.value).toEqual(earlier);
    expect(component.form.controls.end.value).toEqual(later);
  });
});
