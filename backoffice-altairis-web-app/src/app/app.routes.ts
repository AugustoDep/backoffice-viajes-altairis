import { Routes } from '@angular/router';
import { HotelListComponent } from './components/hotel-list/hotel-list.component';
import { HotelDetailComponent } from './components/hotel-detail/hotel-detail.component';
import { LogsComponent } from './components/logs/logs.component';
import { AvailabilityDashboardComponent } from './components/availability-dashboard/availability-dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: '/hotels', pathMatch: 'full' },
  { path: 'hotels', component: HotelListComponent },
  { path: 'hotels/:id', component: HotelDetailComponent },
  { path: 'availability', component: AvailabilityDashboardComponent },
  { path: 'logs', component: LogsComponent },
  { path: '**', redirectTo: '/hotels' }
];