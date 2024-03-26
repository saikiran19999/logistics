import {Address} from '../address';

export interface TruckGeolocation {
  truckId: string;
  tenantId?: string;
  latitude: number;
  longitude: number;
  currentAddress?: Address;
  truckNumber?: string;
  driversName?: string;
}
