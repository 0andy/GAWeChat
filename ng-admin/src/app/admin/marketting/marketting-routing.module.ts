import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { EmployeesComponent } from "./employees/employees.component";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { ActivityComponent } from "./activity/activity.component";
import { ExperienceShareComponent } from "./experience-share/experience-share.component";
import { ContributeManagementComponent } from "./contribute-management/contribute-management.component";
import { ActivityDetailComponent } from "./activity/activity-detail/activity-detail.component";
import { ExperienceDetailComponent } from "./experience-share/experience-detail/experience-detail.component";

const routes: Routes = [
  { path: '', redirectTo: 'index', pathMatch: 'full' },
  { path: 'employee', component: EmployeesComponent, data: { translate: 'employee', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'activity', component: ActivityComponent, data: { translate: 'activity', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'activity-detail', component: ActivityDetailComponent, data: { translate: 'activity', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'activity-detail/:id', component: ActivityDetailComponent, data: { translate: 'activity', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'experience-share', component: ExperienceShareComponent, data: { translate: 'experience-share', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'experience-detail', component: ExperienceDetailComponent, data: { translate: 'experience-share', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'experience-detail/:id', component: ExperienceDetailComponent, data: { translate: 'experience-share', permission: 'Pages' }, canActivate: [AppRouteGuard] },
  { path: 'contribute-management', component: ContributeManagementComponent, data: { translate: 'contribute-management', permission: 'Pages' }, canActivate: [AppRouteGuard] },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarkettingRoutingModule { }