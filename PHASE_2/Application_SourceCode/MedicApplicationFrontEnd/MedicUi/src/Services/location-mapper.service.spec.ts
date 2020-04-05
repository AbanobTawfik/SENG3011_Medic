import { TestBed } from '@angular/core/testing';

import { LocationMapperService } from './location-mapper.service';

describe('LocationMapperService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LocationMapperService = TestBed.get(LocationMapperService);
    expect(service).toBeTruthy();
  });
});
