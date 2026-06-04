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
import { ProjectResourcesComponent } from './features/dashboard/project-resources/project-resources.component';
import { ProjectTasksPageComponent } from './features/dashboard/project-tasks-page/project-tasks-page.component';
import { TaskDetailsComponent } from './features/dashboard/task-details/task-details.component';
import { AddTaskComponent } from './features/dashboard/add-task/add-task.component';
import { ProfileComponent } from './features/dashboard/profile/profile.component';
import { ProjectMembersPageComponent } from './features/dashboard/project-members-page/project-members-page.component';
import { MyTasksComponent } from './features/dashboard/my-tasks/my-tasks.component';
import { MyResourcesComponent } from './features/dashboard/my-resources/my-resources.component';
import { AdminUsersComponent } from './features/dashboard/admin-users/admin-users.component';
import { AdminCreateUserComponent } from './features/dashboard/admin-create-user/admin-create-user.component';
import { AdminEditUserComponent } from './features/dashboard/admin-edit-user/admin-edit-user.component';


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
      { path: 'moji-zadaci', component: MyTasksComponent },
      { path: 'moji-resursi', component: MyResourcesComponent },
      { path: 'projekti/:id', component: ProjectDetailsComponent },
      { path: 'projekti/:id/buildovi', component: ProjectBuildsComponent },
      { path: 'projekti/:id/buildovi/novi', component: AddBuildComponent },
      { path: 'projekti/:id/clanovi', component: ProjectMembersPageComponent },
      { path: 'projekti/:id/resursi', component: ProjectResourcesComponent },
      { path: 'projekti/:id/zadaci', component: ProjectTasksPageComponent },
      { path: 'projekti/:id/zadaci/novi', component: AddTaskComponent, canActivate: [adminGuard] },
      { path: 'projekti/:id/zadaci/:zadatakId', component: TaskDetailsComponent },
      { path: 'profil', component: ProfileComponent },
      {
        path: 'admin/projekti/novi',
        component: AddProjectComponent,
        canActivate: [adminGuard]
      },
      {
        path: 'admin/projekti/:id/izmena',
        component: EditProjectComponent,
        canActivate: [adminGuard]
      },
      {
        path: 'admin/korisnici',
        component: AdminUsersComponent,
        canActivate: [adminGuard]
      },
      {
        path: 'admin/korisnici/novi',
        component: AdminCreateUserComponent,
        canActivate: [adminGuard]
      },
      {
        path: 'admin/korisnici/:id/izmena',
        component: AdminEditUserComponent,
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