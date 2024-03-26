import {Routes} from '@angular/router';
import {Permissions} from '@core/enums';
import {AuthGuard} from '@core/guards';
import {ListPaymentsComponent} from './list-payments/list-payments.component';
import {EditPaymentComponent} from './edit-payment/edit-payment.component';
import {ListInvoicesComponent} from './list-invoices/list-invoices.component';
import {ViewInvoiceComponent} from './view-invoice/view-invoice.component';
import {ListPayrollComponent} from './list-payroll/list-payroll.component';
import {EditPayrollComponent} from './edit-payroll/edit-payroll.component';
import {ViewEmployeePayrollsComponent} from './view-employee-payrolls/view-employee-payrolls.component';


export const ACCOUNTING_ROUTES: Routes = [
  {
    path: 'payments',
    component: ListPaymentsComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Payments',
      permission: Permissions.Payments.View,
    },
  },
  {
    path: 'payments/add',
    component: EditPaymentComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Add',
      permission: Permissions.Payments.Create,
    },
  },
  {
    path: 'payments/edit/:id',
    component: EditPaymentComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Edit',
      permission: Permissions.Payments.Edit,
    },
  },
  {
    path: 'invoices',
    component: ListInvoicesComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Invoices',
      permission: Permissions.Invoices.View,
    },
  },
  {
    path: 'invoices/view/:id',
    component: ViewInvoiceComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'View Invoice',
      permission: Permissions.Invoices.View,
    },
  },
  {
    path: 'payrolls',
    component: ListPayrollComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Payrolls',
      permission: Permissions.Payrolls.View,
    },
  },
  {
    path: 'payrolls/add',
    component: EditPayrollComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Add Payroll',
      permission: Permissions.Payrolls.Create,
    },
  },
  {
    path: 'payrolls/edit/:id',
    component: EditPayrollComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'Edit Payroll',
      permission: Permissions.Payrolls.Edit,
    },
  },
  {
    path: 'employee-payrolls/:employeeId',
    component: ViewEmployeePayrollsComponent,
    canActivate: [AuthGuard],
    data: {
      breadcrumb: 'View Employee Payrolls',
      permission: Permissions.Payrolls.View,
    },
  },
];
