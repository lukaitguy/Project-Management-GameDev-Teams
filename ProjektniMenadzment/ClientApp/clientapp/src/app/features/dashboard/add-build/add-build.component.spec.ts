import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBuildComponent } from './add-build.component';

describe('AddBuildComponent', () => {
  let component: AddBuildComponent;
  let fixture: ComponentFixture<AddBuildComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddBuildComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddBuildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
