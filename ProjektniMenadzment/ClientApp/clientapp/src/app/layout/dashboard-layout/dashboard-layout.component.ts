import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { DashboardHeaderComponent } from '../../shared/components/dashboard-header/dashboard-header.component';
import { DashboardSidebarComponent } from '../../shared/components/dashboard-sidebar/dashboard-sidebar.component';

@Component({
  selector: 'app-dashboard-layout',
  imports: [RouterOutlet, DashboardHeaderComponent, DashboardSidebarComponent],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.scss'
})
export class DashboardLayoutComponent {

}
