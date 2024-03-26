﻿using Logistics.Application.Tenant.Extensions;
using Logistics.Application.Tenant.Services;

namespace Logistics.Application.Tenant.Commands;

internal sealed class CreateLoadHandler : RequestHandler<CreateLoadCommand, ResponseResult>
{
    private readonly ITenantUnityOfWork _tenantUow;
    private readonly IPushNotificationService _pushNotificationService;

    public CreateLoadHandler(
        ITenantUnityOfWork tenantUow,
        IPushNotificationService pushNotificationService)
    {
        _tenantUow = tenantUow;
        _pushNotificationService = pushNotificationService;
    }

    protected override async Task<ResponseResult> HandleValidated(
        CreateLoadCommand req, CancellationToken cancellationToken)
    {
        var dispatcher = await _tenantUow.Repository<Employee>().GetByIdAsync(req.AssignedDispatcherId);

        if (dispatcher is null)
        {
            return ResponseResult.CreateError("Could not find the specified dispatcher");
        }

        var truck = await _tenantUow.Repository<Truck>().GetByIdAsync(req.AssignedTruckId);

        if (truck is null)
        {
            return ResponseResult.CreateError($"Could not find the truck with ID '{req.AssignedTruckId}'");
        }
        
        var customer = await _tenantUow.Repository<Customer>().GetByIdAsync(req.CustomerId);

        if (customer is null)
        {
            return ResponseResult.CreateError($"Could not find the customer with ID '{req.CustomerId}'");
        }
        
        var latestLoad = _tenantUow.Repository<Load>().Query().OrderBy(i => i.RefId).LastOrDefault();
        ulong refId = 1000;

        if (latestLoad is not null)
        {
            refId = latestLoad.RefId + 1;
        }
        
        var load = Load.Create(
            refId,
            req.DeliveryCost,
            req.OriginAddress!,
            req.OriginAddressLat,
            req.OriginAddressLong,
            req.DestinationAddress!,
            req.DestinationAddressLat,
            req.DestinationAddressLong,
            customer,
            truck, 
            dispatcher);
        
        load.Name = req.Name;
        load.Distance = req.Distance;

        await _tenantUow.Repository<Load>().AddAsync(load);
        var changes = await _tenantUow.SaveChangesAsync();

        if (changes > 0)
        {
            await _pushNotificationService.SendNewLoadNotificationAsync(load, truck);
        }
        return ResponseResult.CreateSuccess();
    }
}
