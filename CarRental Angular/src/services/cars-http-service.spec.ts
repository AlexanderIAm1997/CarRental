import { TestBed } from '@angular/core/testing';

import { CarsHttpService } from './cars-http-service';

describe('CarsHttpService', () => {
  let service: CarsHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CarsHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
