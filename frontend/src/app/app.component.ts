import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChatService } from './services/chat.service';
import { MyAuthService } from './services/auth-services/my-auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  standalone: false
})
export class AppComponent implements OnInit {
  languages = [
    { code: 'bs', label: 'Bosanski' },
    { code: 'en', label: 'English' }
  ];

  title = 'frontend';

  constructor(
    private translate: TranslateService,
    private authService: MyAuthService,
    private chatService: ChatService
  ) {
    this.translate.setDefaultLang('en');
    this.translate.use('en');
  }

  changeLanguage(lang: string): void {
    this.translate.use(lang);
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.chatService.startConnection();
    }
  }
}
