export interface Auction {
  id: number;
  title: string;
  description: string;
  startingPrice: number;
  currentPrice: number;
  endTime: string;
  status: string;
  timeRemainingSeconds: number;
}
