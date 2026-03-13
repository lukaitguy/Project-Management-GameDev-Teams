import { TestBed } from '@angular/core/testing';

import { ProjektiService } from './projekti.service';

describe('ProjektiService', () => {
  let service: ProjektiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProjektiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
