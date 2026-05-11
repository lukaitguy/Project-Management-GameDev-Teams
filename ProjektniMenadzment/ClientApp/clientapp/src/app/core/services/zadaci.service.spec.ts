import { TestBed } from '@angular/core/testing';

import { ZadaciService } from './zadaci.service';

describe('ZadaciService', () => {
  let service: ZadaciService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ZadaciService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
