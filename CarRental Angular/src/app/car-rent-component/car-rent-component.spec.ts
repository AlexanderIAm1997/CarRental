import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarRentComponent } from './car-rent-component';

describe('CarRentComponent', () => {
  let component: CarRentComponent;
  let fixture: ComponentFixture<CarRentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CarRentComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CarRentComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
