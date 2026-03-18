import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Auction } from '../../models/auction.model';
import { AuctionService } from '../../services/auction.service';
import { SignalrService } from '../../services/signalr.service';

@Component({
  selector: 'app-auction-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './auction-detail.component.html',
  styleUrl: './auction-detail.component.scss'
})
export class AuctionDetailComponent implements OnInit, OnDestroy {
  auction: Auction | null = null;
  isLoading = false;
  isSubmitting = false;
  bidForm: FormGroup;
  auctionId!: number;
  private countdownInterval: ReturnType<typeof setInterval> | null = null;

  constructor(
    private route: ActivatedRoute,
    private auctionService: AuctionService,
    private signalrService: SignalrService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {
    this.bidForm = this.fb.group({
      bidderName: ['', Validators.required],
      amount: [null, [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.auctionId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadAuction();
    this.setupSignalR();
  }

  loadAuction(): void {
    this.isLoading = true;
    this.auctionService.getAuctionById(this.auctionId).subscribe({
      next: (data) => {
        this.auction = data;
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
    this.signalrService.startConnection().then(() => {
      this.signalrService.joinAuction(this.auctionId);
    });

    this.signalrService.onBidPlaced((data) => {
      if (this.auction) {
        this.auction.currentPrice = data.newPrice;
        this.cdr.detectChanges();
        this.snackBar.open(`${data.bidderName} הציע ₪${data.newPrice}`, 'סגור', { duration: 3000 });
      }
    });

    this.signalrService.onAuctionClosed((data) => {
      if (this.auction) {
        this.auction.status = 'Closed';
        this.cdr.detectChanges();
        this.snackBar.open(`המכירה נסגרה! מחיר סופי: ₪${data.finalPrice}`, 'סגור', { duration: 5000 });
      }
    });
  }

  startCountdown(): void {
    this.countdownInterval = setInterval(() => {
      if (this.auction && this.auction.timeRemainingSeconds > 0) {
        this.auction.timeRemainingSeconds--;
        this.cdr.detectChanges();
      }
    }, 1000);
  }

  placeBid(): void {
    if (this.bidForm.invalid || !this.auction) return;

    const amount = this.bidForm.value.amount;

    if (amount <= this.auction.currentPrice) {
      this.snackBar.open(`ההצעה חייבת להיות גבוהה מ-₪${this.auction.currentPrice}`, 'סגור', { duration: 3000 });
      return;
    }

    if (this.auction.status === 'Closed') {
      this.snackBar.open('המכירה נסגרה', 'סגור', { duration: 3000 });
      return;
    }

    this.isSubmitting = true;
    this.auctionService.placeBid(this.auctionId, this.bidForm.value).subscribe({
      next: () => {
        this.snackBar.open('ההצעה התקבלה!', 'סגור', { duration: 3000 });
        this.bidForm.reset();
        this.isSubmitting = false;
      },
      error: (err) => {
        this.snackBar.open(err.error || 'שגיאה בהגשת ההצעה', 'סגור', { duration: 3000 });
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  formatTime(seconds: number): string {
    if (seconds <= 0) return 'נסגרה';
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  ngOnDestroy(): void {
    this.signalrService.leaveAuction(this.auctionId);
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }
}