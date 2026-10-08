import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrandsProductComponent } from './brands-products.component';

describe('BrandsProductsComponent', () => {
  let component: BrandsProductComponent;
  let fixture: ComponentFixture<BrandsProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BrandsProductComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BrandsProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
