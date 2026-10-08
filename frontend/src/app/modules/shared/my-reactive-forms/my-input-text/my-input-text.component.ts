import {Component, Input, OnInit} from '@angular/core';
import {ControlContainer} from '@angular/forms';
import {MyBaseFormControlComponent} from '../my-base-form-control-component';

export enum MyInputTextType {
  Text = 'text',
  Password = 'password',
  Email = 'email',
  Number = 'number',
  Tel = 'tel',
  Url = 'url'
}

@Component({
  selector: 'app-my-input-text',
  standalone: false,
  templateUrl: './my-input-text.component.html',
  styleUrl: './my-input-text.component.css'
})
export class MyInputTextComponent extends MyBaseFormControlComponent implements OnInit {
  @Input() myLabel!: string;
  @Input() myId = '';
  @Input() myPlaceholder = '';
  @Input() myType: MyInputTextType = MyInputTextType.Text;
  @Input() override customMessages: Record<string, string> = {};
  @Input() override myControlName = '';

  constructor(protected override controlContainer: ControlContainer) {
    super(controlContainer);
  }

  ngOnInit(): void {
    if (!this.myId && this.formControl) {
      this.myId = this.getControlName();
    }
  }
}
