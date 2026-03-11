import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';
import { HomeComponent } from './features/public/home/home.component';
import { AboutComponent } from './features/public/about/about.component';
import { ContactComponent } from './features/public/contact/contact.component';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { HomeComponent as DashboardHomeComponent } from './features/dashboard/home/home.component';
import { DashboardLayoutComponent } from './layout/dashboard-layout/dashboard-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
    {
        path: '',
        component: PublicLayoutComponent,
        children: [
            {
                path: '', component: HomeComponent
            },
            {
                path: 'o-nama', component: AboutComponent
            },
            {
                path: 'kontakt', component: ContactComponent
            }
        ]
    },
    {
        path: '',
        component: AuthLayoutComponent,
        canActivate: [guestGuard],
        children: [
            {
                path: 'prijava', component: LoginComponent
            },
            {
                path: 'registracija', component: RegisterComponent
            }
        ]
    },
    {
        path: '',
        component: DashboardLayoutComponent,
        canActivate: [authGuard],
        children: [
            { path: 'dashboard', component: DashboardHomeComponent }
        ]
    }
];
