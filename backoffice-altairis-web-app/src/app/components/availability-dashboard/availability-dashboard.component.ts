import { Component, OnInit, signal, computed, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Hotel {
  id: number;
  name: string;
  city: string;
  country: string;
  stars: number;
}

interface MonthlyStats {
  month: string;
  year: number;
  totalRooms: number;
  bookedRooms: number;
  availableRooms: number;
  occupancyRate: number;
}

@Component({
  selector: 'app-availability-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './availability-dashboard.component.html',
  styleUrls: ['./availability-dashboard.component.scss']
})
export class AvailabilityDashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5120/api';
  
  Math = Math;
  
  // Data signals
  rawMonthlyStats = signal<MonthlyStats[]>([]);
  hotels = signal<Hotel[]>([]);
  summary = signal<any>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  allHotels = signal<Hotel[]>([]);
  
  // Filter signals
  searchTerm = signal('');
  selectedCountry = signal('');
  selectedCity = signal('');
  selectedYear = signal(new Date().getFullYear());
  
  // Year range: 10 years back, 5 years forward
  yearRange = computed(() => {
    const currentYear = new Date().getFullYear();
    const startYear = currentYear - 10;
    const endYear = currentYear + 5;
    const years: number[] = [];
    for (let year = startYear; year <= endYear; year++) {
      years.push(year);
    }
    return years;
  });

    // Keep original countries list (never filtered)
  allCountries = computed(() => {
    const hotelsList = this.allHotels();
    return [...new Set(hotelsList.map(h => h.country))].sort();
  });
  
  // Keep original cities list based on original hotels
  allCities = computed(() => {
    const hotelsList = this.allHotels();
    if (this.selectedCountry()) {
      // Only filter cities for the "cities" dropdown based on selected country
      return [...new Set(hotelsList
        .filter(h => h.country === this.selectedCountry())
        .map(h => h.city))].sort();
    }
    return [...new Set(hotelsList.map(h => h.city))].sort();
  });
  
  // Available filter options
  countries = computed(() => {
    const hotelsList = this.hotels();
    return [...new Set(hotelsList.map(h => h.country))].sort();
  });
  
  cities = computed(() => {
    const hotelsList = this.hotels();
    let filtered = hotelsList;
    
    if (this.selectedCountry()) {
      filtered = filtered.filter(h => h.country === this.selectedCountry());
    }
    
    return [...new Set(filtered.map(h => h.city))].sort();
  });
  
  // Filtered monthly stats by year
  filteredStats = computed(() => {
    const stats = this.rawMonthlyStats();
    const filtered = stats.filter(s => s.year === this.selectedYear());
    return filtered;
  });
  
  // Max value for chart scaling
  maxChartValue = computed(() => {
    const stats = this.filteredStats();
    if (stats.length === 0) return 50;
    const maxTotal = Math.max(...stats.map(s => s.totalRooms), 10);
    return Math.ceil(maxTotal * 1.1);
  });
  
  // All months
  allMonths = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  
  // Chart data
  chartData = computed(() => {
    const stats = this.filteredStats();
    
    const monthMapping: { [key: string]: string } = {
      'ene.': 'Jan', 'feb.': 'Feb', 'mar.': 'Mar', 'abr.': 'Apr',
      'may.': 'May', 'jun.': 'Jun', 'jul.': 'Jul', 'ago.': 'Aug',
      'sep.': 'Sep', 'oct.': 'Oct', 'nov.': 'Nov', 'dic.': 'Dec'
    };

    const statsMap = new Map<string, MonthlyStats>();
    stats.forEach(stat => {
      if (stat && stat.month) {
        let normalizedMonth = stat.month;
        
        if (monthMapping[stat.month]) {
          normalizedMonth = monthMapping[stat.month];
        }
        else if (!isNaN(Number(stat.month))) {
          const monthNum = parseInt(stat.month);
          const date = new Date(2000, monthNum - 1, 1);
          normalizedMonth = date.toLocaleString('en-US', { month: 'short' });
        }
        
        const normalizedStat = {
          ...stat,
          month: normalizedMonth
        };
        statsMap.set(normalizedMonth, normalizedStat);
      }
    });
    
    const result = this.allMonths.map(month => {
      const data = statsMap.get(month);
      return {
        month,
        totalRooms: data?.totalRooms ?? 0,
        bookedRooms: data?.bookedRooms ?? 0,
        availableRooms: data?.availableRooms ?? 0,
        occupancyRate: data?.occupancyRate ?? 0,
        hasData: !!data
      };
    });
    
    return result;
  });

  // Summary stats for the selected year (used for chart only, not displayed as cards)
  summaryStats = computed(() => {
    const stats = this.filteredStats();
    const totalRooms = stats.reduce((sum, s) => sum + s.totalRooms, 0);
    const totalBooked = stats.reduce((sum, s) => sum + s.bookedRooms, 0);
    const avgOccupancy = stats.length > 0 
      ? stats.reduce((sum, s) => sum + s.occupancyRate, 0) / stats.length 
      : 0;
    
    return {
      totalRooms,
      totalBooked,
      totalAvailable: totalRooms - totalBooked,
      avgOccupancy
    };
  });
  
  ngOnInit(): void {
    this.loadAvailabilityData(this.selectedYear());
  }

  loadAvailabilityData(year?: number, country?: string, city?: string): void {
    this.loading.set(true);
    this.error.set(null);
    
    const yearToFetch = year !== undefined ? year : this.selectedYear();
    const countryToFetch = country !== undefined ? country : this.selectedCountry();
    const cityToFetch = city !== undefined ? city : this.selectedCity();
    
    let url = `${this.apiUrl}/availability/dashboard?year=${yearToFetch}`;
    
    if (countryToFetch && countryToFetch !== '') {
      url += `&country=${encodeURIComponent(countryToFetch)}`;
    }
    if (cityToFetch && cityToFetch !== '') {
      url += `&city=${encodeURIComponent(cityToFetch)}`;
    }
    
    this.http.get<any>(url)
      .subscribe({
        next: (data) => {
          const mappedStats: MonthlyStats[] = (data.monthlyStats || []).map((stat: any) => ({
            month: stat.month || stat.Month,
            year: stat.year || stat.Year,
            totalRooms: stat.totalRooms || stat.TotalRooms || 0,
            bookedRooms: stat.bookedRooms || stat.BookedRooms || 0,
            availableRooms: stat.availableRooms || stat.AvailableRooms || 0,
            occupancyRate: stat.occupancyRate || stat.OccupancyRate || 0
          }));
          
          this.rawMonthlyStats.set(mappedStats);
          
          const hotelsList = data.hotels || [];
          
          // Store original hotels ONLY on first load (when no filters are applied)
          // OR when the original list is empty
          const isFirstLoad = this.allHotels().length === 0;
          const isNoFilters = !countryToFetch && !cityToFetch;
          
          if (isFirstLoad || isNoFilters) {
            this.allHotels.set(hotelsList);
          }
          
          // For display, use the filtered hotels (from API response)
          this.hotels.set(hotelsList);
          this.summary.set(data.summary);
          
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error loading availability data:', err);
          this.error.set('Failed to load availability data. Please try again.');
          this.loading.set(false);
        }
      });
  }
    
  onCountryChange(event: any): void {
    const newCountry = event.target.value;
    this.selectedCountry.set(newCountry);
    this.selectedCity.set(''); // Reset city when country changes
    // Reload data with new filters
    this.loadAvailabilityData(this.selectedYear(), newCountry, '');
  }

  onCityChange(event: any): void {
    const newCity = event.target.value;
    this.selectedCity.set(newCity);
    // Reload data with new filters
    this.loadAvailabilityData(this.selectedYear(), this.selectedCountry(), newCity);
  }

  onYearChange(event: any): void {
    const newYear = parseInt(event.target.value);
    this.selectedYear.set(newYear);
    // Reload data with new filters
    this.loadAvailabilityData(newYear, this.selectedCountry(), this.selectedCity());
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedCountry.set('');
    this.selectedCity.set('');
    const currentYear = new Date().getFullYear();
    this.selectedYear.set(currentYear);
    // Reload data with cleared filters
    this.loadAvailabilityData(currentYear, '', '');
  }
  
  getOccupancyColor(rate: number): string {
    if (rate >= 80) return '#dc3545';
    if (rate >= 60) return '#fd7e14';
    if (rate >= 40) return '#ffc107';
    if (rate >= 20) return '#28a745';
    return '#6c757d';
  }
  
  getBarHeight(value: number): number {
    const max = this.maxChartValue();
    if (max === 0) return 2;
    if (value === 0) return 2;
    const height = (value / max) * 200;
    return Math.max(2, Math.min(200, height));
  }
    
  exportToCSV(): void {
    const stats = this.filteredStats();
    const headers = ['Month', 'Year', 'Total Rooms', 'Booked Rooms', 'Available Rooms', 'Occupancy Rate (%)'];
    const rows = stats.map(s => [
      s.month,
      s.year,
      s.totalRooms,
      s.bookedRooms,
      s.availableRooms,
      s.occupancyRate.toFixed(1)
    ]);
    
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n');
    
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `availability_${this.selectedYear()}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}