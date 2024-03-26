import {HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Tenant} from '@core/models';
import {CookieService} from './cookie.service';


@Injectable({providedIn: 'root'})
export class TenantService {
  private tenant: Tenant | null;

  constructor(private cookieService: CookieService) {
    this.tenant = null;
  }

  getTenantData(): Tenant | null {
    return this.tenant;
  }

  setTenantData(value: Tenant) {
    if (this.tenant === value) {
      return;
    }

    this.tenant = value;
  }

  getTenantName(): string {
    const urlParams = new URLSearchParams(window.location.search);
    const tenantSubDomain = this.getSubDomain(location.host);
    const tenantQuery = urlParams.get('tenant');
    const tenantCookie = this.cookieService.getCookie('X-Tenant');
    let tenantName = 'default';

    if (tenantSubDomain) {
      tenantName = tenantSubDomain;
    }
    else if (tenantQuery) {
      tenantName = tenantQuery;
    }
    else if (tenantCookie) {
      tenantName = tenantCookie;
    }

    if (tenantName === 'office') {
      tenantName = 'default';
    }

    return tenantName;
  }

  createTenantHeaders(headers: HttpHeaders, tenantName: string): HttpHeaders {
    return headers.append('X-Tenant', tenantName);
  }

  setTenantCookie(tenantName: string) {
    if (!tenantName) {
      return;
    }

    const currentTenant = this.cookieService.getCookie('X-Tenant');

    if (tenantName === currentTenant) {
      return;
    }

    this.cookieService.setCookie({
      name: 'X-Tenant',
      value: tenantName,
      session: true,
    });
  }

  private getSubDomain(host: string) {
    let subDomain = '';
    const domains = host.split('.');

    if (domains.length <= 2)
    {return subDomain;}

    subDomain = domains[0];
    return subDomain;
  }
}
