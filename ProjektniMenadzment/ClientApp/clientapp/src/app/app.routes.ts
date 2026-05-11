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
import { adminGuard } from './core/guards/admin.guard';
import { MyProjectsComponent } from './features/dashboard/my-projects/my-projects.component';
import { ProjectDetailsComponent } from './features/dashboard/project-details/project-details.component';
import { ProjectBuildsComponent } from './features/dashboard/project-builds/project-builds.component';
import { AddBuildComponent } from './features/dashboard/add-build/add-build.component';
import { AddProjectComponent } from './features/dashboard/add-project/add-project.component';
import { EditProjectComponent } from './features/dashboard/edit-project/edit-project.component';

export const routes: Routes = [
  // ── Public ────────────────────────────────────────────────
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'o-nama', component: AboutComponent },
      { path: 'kontakt', component: ContactComponent }
    ]
  },

  // ── Auth (guests only) ────────────────────────────────────
  {
    path: '',
    component: AuthLayoutComponent,
    canActivate: [guestGuard],
    children: [
      { path: 'prijava', component: LoginComponent },
      { path: 'registracija', component: RegisterComponent }
    ]
  },

  // ── Dashboard (authenticated users only) ─────────────────
  {
    path: '',
    component: DashboardLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardHomeComponent },
      { path: 'moji-projekti', component: MyProjectsComponent },
      { path: 'projekti/:id', component: ProjectDetailsComponent },
      { path: 'projekti/:id/buildovi', component: ProjectBuildsComponent },
      { path: 'projekti/:id/buildovi/novi', component: AddBuildComponent },
      {
        path: 'admin/projekti/novi',
        component: AddProjectComponent,
        canActivate: [adminGuard]
      },
      {
        path: 'admin/projekti/:id/izmena',
        component: EditProjectComponent,
        canActivate: [adminGuard]
      }
    ]
  },

  // ── Fallback ──────────────────────────────────────────────
  {
    path: '**',
    redirectTo: ''
  }
];