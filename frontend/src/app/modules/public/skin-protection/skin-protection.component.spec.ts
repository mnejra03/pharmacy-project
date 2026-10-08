import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkinProtectionComponent } from './skin-protection.component';

describe('SkinProtectionComponent', () => {
  let component: SkinProtectionComponent;
  let fixture: ComponentFixture<SkinProtectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SkinProtectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SkinProtectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
