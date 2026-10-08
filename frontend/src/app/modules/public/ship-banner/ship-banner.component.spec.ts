import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShipBannerComponent } from './ship-banner.component';

describe('ShipBannerComponent', () => {
  let component: ShipBannerComponent;
  let fixture: ComponentFixture<ShipBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ShipBannerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShipBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
