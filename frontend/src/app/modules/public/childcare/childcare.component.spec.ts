import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildcareComponent } from './childcare.component';

describe('ChildcareComponent', () => {
  let component: ChildcareComponent;
  let fixture: ComponentFixture<ChildcareComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ChildcareComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChildcareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
