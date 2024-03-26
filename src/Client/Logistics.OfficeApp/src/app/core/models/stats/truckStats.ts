import {Employee} from '../employee/employee';

export interface TruckStats {
  truckId: string;
  truckNumber: string;
  startDate: string;
  endDate: string;
  gross: number;
  distance: number;
  driverShare: number;
  drivers: Employee[];
}
