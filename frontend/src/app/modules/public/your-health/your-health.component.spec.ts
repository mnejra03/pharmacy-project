import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YourHealthComponent } from './your-health.component';

describe('YourHealthComponent', () => {
  let component: YourHealthComponent;
  let fixture: ComponentFixture<YourHealthComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [YourHealthComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(YourHealthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
