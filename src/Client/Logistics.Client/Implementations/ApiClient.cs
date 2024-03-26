﻿using System.IdentityModel.Tokens.Jwt;
using Logistics.Client.Exceptions;
using Logistics.Client.Models;
using Logistics.Shared.Models;
using Logistics.Client.Options;
using Logistics.Shared;

namespace Logistics.Client.Implementations;

internal class ApiClient : GenericApiClient, IApiClient
{
    private string? _accessToken;
    private string? _tenantId;
    
    public ApiClient(ApiClientOptions options) : base(options.Host!)
    {
        AccessToken = options.AccessToken;
    }

    public event EventHandler<string>? OnErrorResponse;

    public string? AccessToken
    {
        get => _accessToken;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            _accessToken = value;
            SetAuthorizationHeader("Bearer", _accessToken);
            SetTenantIdFromAccessToken(_accessToken);
        }
    }

    public string? TenantId 
    {
        get => _tenantId;
        set
        {
            _tenantId = value;
            SetRequestHeader("X-Tenant", _tenantId);
        }
    }
    
    private void SetTenantIdFromAccessToken(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return;
        }
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var tenantId = token?.Claims?.FirstOrDefault(i => i.Type == "tenant")?.Value;
        
        if (string.IsNullOrEmpty(tenantId) || TenantId == tenantId)
        {
            return;
        } 

        TenantId = tenantId;
        SetRequestHeader("X-Tenant", tenantId);
    }

    private async Task<TRes> MakeGetRequestAsync<TRes>(string endpoint, IDictionary<string, string>? query = null)
        where TRes : class, IResponseResult, new()
    {
        try
        {
            var result = await GetRequestAsync<TRes>(endpoint, query);

            if (!result.IsSuccess)
            {
                OnErrorResponse?.Invoke(this, result.Error!);
            }

            return result;
        }
        catch (ApiException ex)
        {
            OnErrorResponse?.Invoke(this, ex.Message);
            return new TRes { Error = ex.Message };
        }
    }

    private async Task<TRes> MakePostRequestAsync<TRes, TBody>(string endpoint, TBody body)
        where TRes : class, IResponseResult, new()
        where TBody : class, new()
    {
        try
        {
            var result = await PostRequestAsync<TRes, TBody>(endpoint, body);

            if (!result.IsSuccess)
            {
                OnErrorResponse?.Invoke(this, result.Error!);
            }

            return result;
        }
        catch (ApiException ex)
        {
            OnErrorResponse?.Invoke(this, ex.Message);
            return new TRes { Error = ex.Message };
        }
    }

    private async Task<TRes> MakePutRequestAsync<TRes, TBody>(string endpoint, TBody body)
        where TRes : class, IResponseResult, new()
        where TBody : class, new()
    {
        try
        {
            var result = await PutRequestAsync<TRes, TBody>(endpoint, body);

            if (!result.IsSuccess)
            {
                OnErrorResponse?.Invoke(this, result.Error!);
            }

            return result;
        }
        catch (ApiException ex)
        {
            OnErrorResponse?.Invoke(this, ex.Message);
            return new TRes { Error = ex.Message };
        }
    }

    private async Task<TRes> MakeDeleteRequestAsync<TRes>(string endpoint)
        where TRes : class, IResponseResult, new()
    {
        try
        {
            var result = await DeleteRequestAsync<TRes>(endpoint);

            if (!result.IsSuccess)
            {
                OnErrorResponse?.Invoke(this, result.Error!);
            }

            return result;
        }
        catch (ApiException ex)
        {
            OnErrorResponse?.Invoke(this, ex.Message);
            return new TRes { Error = ex.Message };
        }
    }


    #region Load API
    
    public Task<ResponseResult<LoadDto>> GetLoadAsync(string id)
    {
        return MakeGetRequestAsync<ResponseResult<LoadDto>>($"loads/{id}");
    }

    public Task<PagedResponseResult<LoadDto>> GetLoadsAsync(GetLoadsQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<LoadDto>>("loads", query.ToDictionary());
    }
    
    public Task<ResponseResult<ICollection<LoadDto>>> GetDriverActiveLoadsAsync(string userId)
    {
        var query = new Dictionary<string, string>
        {
            { "userId", userId },
            { "onlyActiveLoads", "true" },
            { "loadAllPage", "true" }
        };
        return MakeGetRequestAsync<ResponseResult<ICollection<LoadDto>>>("loads", query);
    }

    public Task<ResponseResult> CreateLoadAsync(CreateLoad command)
    {
        return MakePostRequestAsync<ResponseResult, CreateLoad>("loads", command);
    }
    
    public Task<ResponseResult> UpdateLoadAsync(UpdateLoad command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateLoad>($"loads/{command.Id}", command);
    }

    public Task<ResponseResult> DeleteLoadAsync(string id)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"loads/{id}");
    }

    #endregion


    #region Truck API

    public Task<ResponseResult<TruckDto>> GetTruckAsync(GetTruckQuery query)
    {
        var id = query.TruckOrDriverId;
        return MakeGetRequestAsync<ResponseResult<TruckDto>>($"trucks/{id}", query.ToDictionary());
    }

    public Task<PagedResponseResult<TruckDto>> GetTrucksAsync(SearchableQuery query, bool includeLoads = false)
    {
        var queryDict = query.ToDictionary();
        queryDict.Add("includeLoads", includeLoads.ToString());
        return MakeGetRequestAsync<PagedResponseResult<TruckDto>>("trucks", queryDict);
    }

    public Task<ResponseResult> CreateTruckAsync(CreateTruck command)
    {
        return MakePostRequestAsync<ResponseResult, CreateTruck>("trucks", command);
    }

    public Task<ResponseResult> UpdateTruckAsync(UpdateTruck command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateTruck>($"trucks/{command.Id}", command);
    }

    public Task<ResponseResult> DeleteTruckAsync(string id)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"trucks/{id}");
    }

    #endregion


    #region Employee API

    public Task<ResponseResult<EmployeeDto>> GetEmployeeAsync(string userId)
    {
        return MakeGetRequestAsync<ResponseResult<EmployeeDto>>($"employees/{userId}");
    }

    public Task<PagedResponseResult<EmployeeDto>> GetEmployeesAsync(SearchableQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<EmployeeDto>>("employees", query.ToDictionary());
    }

    public Task<ResponseResult> CreateEmployeeAsync(CreateEmployee command)
    {
        return MakePostRequestAsync<ResponseResult, CreateEmployee>("employees", command);
    }

    public Task<ResponseResult> UpdateEmployeeAsync(UpdateEmployee command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateEmployee>($"employees/{command.UserId}", command);
    }
    
    public Task<ResponseResult> DeleteEmployeeAsync(string userId)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"employees/{userId}");
    }

    #endregion


    #region Tenant API

    public Task<ResponseResult<TenantDto>> GetTenantAsync(string identifier)
    {
        return MakeGetRequestAsync<ResponseResult<TenantDto>>($"tenants/{identifier}");
    }

    public Task<PagedResponseResult<TenantDto>> GetTenantsAsync(SearchableQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<TenantDto>>("tenants", query.ToDictionary());
    }

    public Task<ResponseResult> CreateTenantAsync(CreateTenant command)
    {
        return MakePostRequestAsync<ResponseResult, CreateTenant>("tenants", command);
    }

    public Task<ResponseResult> UpdateTenantAsync(UpdateTenant command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateTenant>($"tenants/{command.Id}", command);
    }

    public Task<ResponseResult> DeleteTenantAsync(string id)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"tenants/{id}");
    }

    #endregion


    #region User API

    public Task<ResponseResult<UserDto>> GetUserAsync(string userId)
    {
        return MakeGetRequestAsync<ResponseResult<UserDto>>($"users/{userId}");
    }

    public Task<PagedResponseResult<UserDto>> GetUsersAsync(SearchableQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<UserDto>>("users", query.ToDictionary());
    }

    public Task<ResponseResult> UpdateUserAsync(UpdateUser command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateUser>($"users/{command.UserId}", command);
    }

    public Task<ResponseResult<OrganizationDto[]>> GetUserOrganizations(string userId)
    {
        return MakeGetRequestAsync<ResponseResult<OrganizationDto[]>>($"users/{userId}/organizations");
    }

    #endregion


    #region Driver API
    
    public Task<ResponseResult> SetDeviceTokenAsync(SetDeviceToken command)
    {
        return MakePostRequestAsync<ResponseResult, SetDeviceToken>($"drivers/{command.UserId}/device-token", command);
    }

    public Task<ResponseResult> ConfirmLoadStatusAsync(ConfirmLoadStatus command)
    {
        return MakePostRequestAsync<ResponseResult, ConfirmLoadStatus>("drivers/confirm-load-status", command);
    }

    public Task<ResponseResult> UpdateLoadProximity(UpdateLoadProximity command)
    {
        return MakePostRequestAsync<ResponseResult, UpdateLoadProximity>("drivers/update-load-proximity", command);
    }

    #endregion


    #region Stats API

    public Task<ResponseResult<DailyGrossesDto>> GetDailyGrossesAsync(GetDailyGrossesQuery query)
    {
        return MakeGetRequestAsync<ResponseResult<DailyGrossesDto>>("stats/daily-grosses", query.ToDictionary());
    }

    public Task<ResponseResult<MonthlyGrossesDto>> GetMonthlyGrossesAsync(GetMonthlyGrossesQuery query)
    {
        return MakeGetRequestAsync<ResponseResult<MonthlyGrossesDto>>("stats/monthly-grosses", query.ToDictionary());
    }

    public Task<ResponseResult<DriverStatsDto>> GetDriverStatsAsync(string userId)
    {
        return MakeGetRequestAsync<ResponseResult<DriverStatsDto>>($"stats/driver/{userId}");  
    }

    #endregion

    
    #region Subscriptions API

    public Task<ResponseResult<SubscriptionDto>> GetSubscriptionAsync(string id)
    {
        return MakeGetRequestAsync<ResponseResult<SubscriptionDto>>($"subscriptions/{id}");
    }

    public Task<ResponseResult<SubscriptionPlanDto>> GetSubscriptionPlanAsync(string planId)
    {
        return MakeGetRequestAsync<ResponseResult<SubscriptionPlanDto>>($"subscriptions/plans/{planId}");
    }

    public Task<PagedResponseResult<SubscriptionDto>> GetSubscriptionsAsync(PagedQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<SubscriptionDto>>("subscriptions", query.ToDictionary());
    }

    public Task<PagedResponseResult<SubscriptionPlanDto>> GetSubscriptionPlansAsync(PagedQuery query)
    {
        return MakeGetRequestAsync<PagedResponseResult<SubscriptionPlanDto>>("subscriptions/plans", query.ToDictionary());
    }

    public Task<ResponseResult> CreateSubscriptionPlanAsync(CreateSubscriptionPlan command)
    {
        return MakePostRequestAsync<ResponseResult, CreateSubscriptionPlan>("subscriptions/plans", command);
    }

    public Task<ResponseResult> UpdateSubscriptionPlanAsync(UpdateSubscriptionPlan command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateSubscriptionPlan>($"subscriptions/plans/{command.Id}", command);
    }

    public Task<ResponseResult> DeleteSubscriptionPlanAsync(string id)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"subscriptions/plans/{id}");
    }

    public Task<ResponseResult> CreateSubscriptionAsync(CreateSubscription command)
    {
        return MakePostRequestAsync<ResponseResult, CreateSubscription>("subscriptions", command);
    }

    public Task<ResponseResult> UpdateSubscriptionAsync(UpdateSubscription command)
    {
        return MakePutRequestAsync<ResponseResult, UpdateSubscription>($"subscriptions/{command.Id}", command);
    }

    public Task<ResponseResult> DeleteSubscriptionAsync(string id)
    {
        return MakeDeleteRequestAsync<ResponseResult>($"subscriptions/{id}");
    }

    #endregion
}
