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

  constructor(private chat: ChatService) { }

  async ngOnInit(): Promise<void> {
    console.log(`SignalR connected`);
    await this.chat.connect();
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
        await this.chat.joinConversation(id);
        this.loadMessages();
      });
  }

    async joinConversation(): Promise<void> {
    const id = this.joinId.trim();
    this.joinId = '';
    try {
      console.log(`[Client] Attempting to join conversation: ${id}`);
      await this.chat.joinConversation(id);
      this.conversationId = id;
      console.log('[Client] Successfully joined the conversation.');
      this.loadMessages();
    } catch (error) {
      console.error('[Client] Failed to join conversation:', error);
    }
  }

  test():void{
    console.log('Test function called');
    // You can add any test logic here
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
