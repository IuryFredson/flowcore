import { Component, computed, signal } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

type ProtocolStatus = 'Draft' | 'Open' | 'InAnalysis' | 'WaitingForDocuments' | 'Approved' | 'Rejected' | 'Closed';

interface ProtocolListItem {
  number: string;
  subject: string;
  status: ProtocolStatus;
  department: string;
  updatedAt: string;
}

@Component({
  selector: 'fc-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app/app.component.html',
  styleUrl: './app/app.component.css'
})
class AppComponent {
  protected readonly protocols = signal<ProtocolListItem[]>([
    {
      number: 'FC-202606080001',
      subject: 'Vehicle registration appeal',
      status: 'InAnalysis',
      department: 'Legal Review',
      updatedAt: '2026-06-08 09:40'
    },
    {
      number: 'FC-202606070014',
      subject: 'Driver license renewal exception',
      status: 'WaitingForDocuments',
      department: 'Citizen Service',
      updatedAt: '2026-06-07 16:15'
    },
    {
      number: 'FC-202606060028',
      subject: 'Internal procurement authorization',
      status: 'Approved',
      department: 'Administration',
      updatedAt: '2026-06-06 11:05'
    }
  ]);

  protected readonly openCount = computed(() =>
    this.protocols().filter(protocol => protocol.status !== 'Closed' && protocol.status !== 'Rejected').length);
}

bootstrapApplication(AppComponent).catch(error => console.error(error));

