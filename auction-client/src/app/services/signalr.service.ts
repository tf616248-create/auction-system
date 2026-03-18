import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .build();
  }

  startConnection(): Promise<void> {
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      return Promise.resolve();
    }

    return this.hubConnection
      .start()
      .catch((err) => console.error('SignalR error:', err));
  }

  joinAuction(auctionId: number): void {
    this.hubConnection.invoke('JoinAuction', auctionId.toString());
  }

  leaveAuction(auctionId: number): void {
    this.hubConnection.invoke('LeaveAuction', auctionId.toString());
  }

  onBidPlaced(
    callback: (data: {
      auctionId: number;
      newPrice: number;
      bidderName: string;
    }) => void,
  ): void {
    this.hubConnection.on('BidPlaced', callback);
  }
  onAuctionClosed(
    callback: (data: { auctionId: number; finalPrice: number }) => void,
  ): void {
    this.hubConnection.on('AuctionClosed', callback);
  }

  offBidPlaced(): void {
    this.hubConnection.off('BidPlaced');
  }

  offAuctionClosed(): void {
    this.hubConnection.off('AuctionClosed');
  }
}
