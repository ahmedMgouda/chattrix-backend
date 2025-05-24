import { Component, OnInit } from '@angular/core';
import { ChatService, ChatMessage, ChatAttachment } from './chat.service';
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
  attachments: ChatAttachment[] = [];
  isRecording = false;
  private mediaRecorder?: MediaRecorder;
  private recordedChunks: BlobPart[] = [];

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

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;
    const files = Array.from(input.files);
    for (const file of files) {
      const data = await this.readFileAsBase64(file);
      this.attachments.push({ fileName: file.name, data });
    }
    // Reset input value to allow selecting the same file again
    input.value = '';
  }

  private readFileAsBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const result = reader.result as string;
        const base64 = result.split(',')[1];
        resolve(base64);
      };
      reader.onerror = () => reject(reader.error);
      reader.readAsDataURL(file);
    });
  }

  private readBlobAsBase64(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const result = reader.result as string;
        const base64 = result.split(',')[1];
        resolve(base64);
      };
      reader.onerror = () => reject(reader.error);
      reader.readAsDataURL(blob);
    });
  }

  async startRecording(): Promise<void> {
    if (this.isRecording) return;
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      this.recordedChunks = [];
      this.mediaRecorder = new MediaRecorder(stream);
      this.mediaRecorder.ondataavailable = e => {
        if (e.data.size > 0) this.recordedChunks.push(e.data);
      };
      this.mediaRecorder.onstop = async () => {
        const blob = new Blob(this.recordedChunks, { type: 'audio/webm' });
        const base64 = await this.readBlobAsBase64(blob);
        const attachment: ChatAttachment = {
          fileName: `voice-${Date.now()}.webm`,
          data: base64
        };
        this.attachments.push(attachment);
        this.mediaRecorder = undefined;
        this.recordedChunks = [];
      };
      this.mediaRecorder.start();
      this.isRecording = true;
    } catch (err) {
      console.error('Failed to start recording', err);
    }
  }

  stopRecording(): void {
    if (!this.mediaRecorder || !this.isRecording) return;
    this.mediaRecorder.stop();
    this.isRecording = false;
  }

  isAudio(file: ChatAttachment): boolean {
    return /\.(mp3|wav|ogg|m4a|webm)$/i.test(file.fileName);
  }

  getFileUrl(file: ChatAttachment): string {
    const ext = file.fileName.split('.').pop()?.toLowerCase() || '';
    const mimeMap: Record<string, string> = {
      mp3: 'audio/mpeg',
      wav: 'audio/wav',
      ogg: 'audio/ogg',
      m4a: 'audio/mp4',
      webm: 'audio/webm'
    };
    const mime = mimeMap[ext] || 'application/octet-stream';
    return `data:${mime};base64,${file.data}`;
  }

  loadMessages(): void {
    if (!this.conversationId) return;
    this.chat.getMessages(this.conversationId)
      .subscribe(ms => this.messages = ms);
  }

  send(): void {
    if (!this.conversationId) return;
    if (!this.newMessage.trim() && this.attachments.length === 0) return;
    this.chat
      .sendMessageViaHub(
        this.conversationId,
        this.user,
        this.newMessage,
        this.attachments.length ? [...this.attachments] : undefined
      )
      .then(() => {
        this.newMessage = '';
        this.attachments = [];
      });
  }
}
