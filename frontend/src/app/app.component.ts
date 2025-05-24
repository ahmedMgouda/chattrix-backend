import { Component, OnInit } from '@angular/core';
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
export class AppComponent implements OnInit {
  title = 'Chat Frontend';
  conversationId: string | null = null;
  user = '';
  recipient = '';
  topic = '';
  joinId = '';
  messages: ChatMessage[] = [];
  newMessage = '';

  constructor(private chat: ChatService) {}

  ngOnInit(): void {
    this.chat.onMessage((convId, sender, content) => {
      if (this.conversationId === convId) {
        this.loadMessages();
      }
    });
  }

  startConversation(): void {
    if (!this.user.trim() || !this.recipient.trim() || !this.topic.trim()) return;
    this.chat.startConversation(this.user, this.recipient, this.topic)
      .subscribe(async id => {
        this.conversationId = id;
        await this.chat.connect();
        await this.chat.joinConversation(id);
        this.loadMessages();
      });
  }

  joinConversation(): void {
    if (!this.user.trim() || !this.joinId.trim()) return;
    this.conversationId = this.joinId;
    this.joinId = '';
    this.chat.connect().then(() => this.chat.joinConversation(this.conversationId!));
    this.loadMessages();
  }

  loadMessages(): void {
    if (!this.conversationId) return;
    this.chat.getMessages(this.conversationId)
      .subscribe(ms => this.messages = ms);
  }

  send(): void {
    if (!this.conversationId || !this.newMessage.trim()) return;
    this.chat.sendMessageViaHub(this.conversationId, this.user, this.newMessage)
      .then(() => {
        this.newMessage = '';
      });
  }
}
