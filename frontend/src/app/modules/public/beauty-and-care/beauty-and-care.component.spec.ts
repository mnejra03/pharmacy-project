import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BeautyAndCareComponent } from './beauty-and-care.component';

describe('BeautyAndCareComponent', () => {
  let component: BeautyAndCareComponent;
  let fixture: ComponentFixture<BeautyAndCareComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BeautyAndCareComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BeautyAndCareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
