import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Auction } from '../models/auction.model';
import { Bid, PlaceBidRequest } from '../models/bid.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuctionService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAuctions(): Observable<Auction[]> {
    return this.http.get<Auction[]>(`${this.apiUrl}/auctions`);
  }

  getAuctionById(id: number): Observable<Auction> {
    return this.http.get<Auction>(`${this.apiUrl}/auctions/${id}`);
  }

  placeBid(auctionId: number, bid: PlaceBidRequest): Observable<Bid> {
    return this.http.post<Bid>(
      `${this.apiUrl}/auctions/${auctionId}/bids`,
      bid,
    );
  }
}
