import { TestBed } from '@angular/core/testing';

import { DateFormatterService } from './date-formatter.service';

describe('DateFormatterService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DateFormatterService = TestBed.get(DateFormatterService);
    expect(service).toBeTruthy();
  });
});
