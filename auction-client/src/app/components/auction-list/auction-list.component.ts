import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Auction } from '../../models/auction.model';
import { AuctionService } from '../../services/auction.service';
import { SignalrService } from '../../services/signalr.service';

@Component({
  selector: 'app-auction-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './auction-list.component.html',
  styleUrl: './auction-list.component.scss'
})
export class AuctionListComponent implements OnInit, OnDestroy {
  auctions: Auction[] = [];
  isLoading = false;
  private countdownInterval: ReturnType<typeof setInterval> | null = null;

  constructor(
    private auctionService: AuctionService,
    private signalrService: SignalrService
  ) {}

  ngOnInit(): void {
    this.loadAuctions();
    this.setupSignalR();
  }

  loadAuctions(): void {
    this.isLoading = true;
    this.auctionService.getAuctions().subscribe({
      next: (data) => {
        this.auctions = data;
        this.isLoading = false;
        this.startCountdown();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  setupSignalR(): void {
    this.signalrService.startConnection().then(() => {});

    this.signalrService.onBidPlaced((data) => {
      const auction = this.auctions.find(a => a.id === data.auctionId);
      if (auction) {
        auction.currentPrice = data.newPrice;
        this.auctions = [...this.auctions];
      }
    });

    this.signalrService.onAuctionClosed((data) => {
      const auction = this.auctions.find(a => a.id === data.auctionId);
      if (auction) {
        auction.status = 'Closed';
        this.auctions = [...this.auctions];
      }
    });
  }

  startCountdown(): void {
    this.countdownInterval = setInterval(() => {
      this.auctions = this.auctions.map(a => ({
        ...a,
        timeRemainingSeconds: Math.max(0, a.timeRemainingSeconds - 1)
      }));
    }, 1000);
  }

  formatTime(seconds: number): string {
    if (seconds <= 0) return 'נסגרה';
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  ngOnDestroy(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
    this.signalrService.offBidPlaced();
    this.signalrService.offAuctionClosed();
  }
}