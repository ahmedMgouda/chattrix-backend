import { Component } from '@angular/core';
import { ChatService, ChatMessage } from './chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [
    CommonModule,    // Needed for *ngIf, *ngFor
    FormsModule      // Needed for [(ngModel)]
  ]
})
export class AppComponent {
  title = 'Chat Frontend';
  conversationId: string | null = null;
  user = '';
  recipient = '';
  topic = '';
  messages: ChatMessage[] = [];
  newMessage = '';

  constructor(private chat: ChatService) {}

  startConversation(): void {
    if (!this.user.trim() || !this.recipient.trim() || !this.topic.trim()) return;
    this.chat.startConversation(this.user, this.recipient, this.topic)
      .subscribe(id => {
        this.conversationId = id;
        this.loadMessages();
      });
  }

  loadMessages(): void {
    if (!this.conversationId) return;
    this.chat.getMessages(this.conversationId)
      .subscribe(ms => this.messages = ms);
  }

  send(): void {
    if (!this.conversationId || !this.newMessage.trim()) return;
    this.chat.sendMessage(this.conversationId, this.user, this.newMessage)
      .subscribe(() => {
        this.newMessage = '';
        this.loadMessages();
      });
  }
}
