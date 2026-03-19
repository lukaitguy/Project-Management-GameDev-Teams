import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectBuildsComponent } from './project-builds.component';

describe('ProjectBuildsComponent', () => {
  let component: ProjectBuildsComponent;
  let fixture: ComponentFixture<ProjectBuildsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectBuildsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectBuildsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
