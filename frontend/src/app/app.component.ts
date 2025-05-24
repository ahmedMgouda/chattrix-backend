import { Component, OnInit } from '@angular/core';
import { ChatService, ChatMessage } from './chat.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Chat Frontend';
  conversationId: string | null = null;
  user = 'alice';
  recipient = 'bob';
  topic = 'general';
  messages: ChatMessage[] = [];
  newMessage = '';

  constructor(private chat: ChatService) {}

  ngOnInit(): void {
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
