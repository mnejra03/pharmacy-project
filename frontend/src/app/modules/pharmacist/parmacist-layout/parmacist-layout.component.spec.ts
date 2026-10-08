import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParmacistLayoutComponent } from './parmacist-layout.component';

describe('ParmacistLayoutComponent', () => {
  let component: ParmacistLayoutComponent;
  let fixture: ComponentFixture<ParmacistLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ParmacistLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ParmacistLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
