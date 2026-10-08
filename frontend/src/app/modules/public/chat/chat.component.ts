import { Component, ElementRef, ViewChild, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { ChatService, ChatGetDTO, ChatCreateDTO } from '../../../services/chat.service';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';
import { UserService, MyAppUserDTO } from '../../../services/user.service';

@Component({
  selector: 'app-chat',
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {
  isOpen = false;
  messageText = '';
  messages: { from: 'user' | 'bot', text: string, date?: Date, senderName?: string }[] = [];
  isLoading = false;

  senderId!: number | null;
  receiverId!: number;

  private msgSub!: Subscription;

  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  constructor(
    private chatService: ChatService,
    private authService: MyAuthService,
    private userService: UserService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.senderId = this.authService.getCurrentUserId();
    if (!this.senderId) {
      return;
    }

    this.msgSub = this.chatService.message$.subscribe((msg) => {
      this.onReceiveMessage(msg);
    });

    if (this.authService.isCustomer()) {
      this.initChatWithPharmacist();
    }
  }

  ngOnDestroy(): void {
    this.msgSub?.unsubscribe();
  }

  private initChatWithPharmacist(): void {
    this.chatService.getLastPharmacist().subscribe({
      next: (response) => {
        if (response && response.pharmacistId) {
          this.receiverId = response.pharmacistId;
          this.loadMessages();
        } else {
          this.loadFirstAvailablePharmacist();
        }
      },
      error: () => {
        this.loadFirstAvailablePharmacist();
      }
    });
  }

  private loadFirstAvailablePharmacist(): void {
    this.userService.getPharmacists().subscribe({
      next: (pharmacists: MyAppUserDTO[]) => {
        if (!pharmacists.length) {
          return;
        }

        this.receiverId = pharmacists[0].id;
      },
      error: () => {}
    });
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.scrollToBottom();
    }
  }

  private loadMessages(): void {
    if (!this.senderId) {
      return;
    }

    this.isLoading = true;

    this.chatService.getUserConversations().subscribe({
      next: (chatMessages: ChatGetDTO[]) => {
        this.messages = chatMessages.map(m => ({
          from: m.senderId === this.senderId ? 'user' : 'bot',
          text: m.message,
          date: m.date,
          senderName: m.senderId === this.senderId ? 'Vi' : m.senderName
        }));
        this.isLoading = false;

        if (this.isOpen) {
          this.scrollToBottom();
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  sendMessage(): void {
    const text = this.messageText.trim();
    if (!text || !this.senderId || !this.receiverId) {
      return;
    }

    const dto: ChatCreateDTO = {
      receiverId: this.receiverId,
      message: text,
      typeOfMessage: 'question',
      status: 'sent',
      isResponse: false
    };

    this.messages.push({ from: 'user', text, date: new Date(), senderName: 'Vi' });
    this.messageText = '';
    this.scrollToBottom();

    this.chatService.sendMessage(dto).subscribe({
      next: () => {},
      error: () => {
        this.messages.pop();
      }
    });
  }

  private onReceiveMessage(msg: any): void {
    if (msg.senderId == this.senderId) {
      return;
    }

    this.messages.push({
      from: 'bot',
      text: msg.message,
      date: new Date(msg.date),
      senderName: 'Farmaceut'
    });
    this.scrollToBottom();
    this.cdr.detectChanges();
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      try {
        if (this.myScrollContainer) {
          this.myScrollContainer.nativeElement.scrollTop =
            this.myScrollContainer.nativeElement.scrollHeight;
        }
      } catch {}
    }, 50);
  }
}
