export interface Bid {
  id: number;
  bidderName: string;
  amount: number;
  placedAt: string;
}

export interface PlaceBidRequest {
  bidderName: string;
  amount: number;
}
