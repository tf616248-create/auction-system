import { Routes } from '@angular/router';
import { AuctionListComponent } from './components/auction-list/auction-list.component';
import { AuctionDetailComponent } from './components/auction-detail/auction-detail.component';

export const routes: Routes = [
  { path: '', component: AuctionListComponent },
  { path: 'auction/:id', component: AuctionDetailComponent },
  { path: '**', redirectTo: '' },
];
