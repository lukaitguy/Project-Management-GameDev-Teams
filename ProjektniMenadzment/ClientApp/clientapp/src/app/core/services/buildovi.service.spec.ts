import { TestBed } from '@angular/core/testing';

import { BuildoviService } from './buildovi.service';

describe('BuildoviService', () => {
  let service: BuildoviService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BuildoviService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
