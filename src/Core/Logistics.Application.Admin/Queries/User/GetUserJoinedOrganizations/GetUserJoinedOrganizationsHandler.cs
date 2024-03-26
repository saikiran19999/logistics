﻿using Logistics.Application.Core;
using Logistics.Domain.Entities;
using Logistics.Domain.Persistence;
using Logistics.Shared.Models;
using Logistics.Shared;

namespace Logistics.Application.Admin.Queries;

internal sealed class GetUserJoinedOrganizationsHandler : 
    RequestHandler<GetUserJoinedOrganizationsQuery, ResponseResult<OrganizationDto[]>>
{
    private readonly IMasterUnityOfWork _masterUow;

    public GetUserJoinedOrganizationsHandler(IMasterUnityOfWork masterUow)
    {
        _masterUow = masterUow;
    }

    protected override async Task<ResponseResult<OrganizationDto[]>> HandleValidated(
        GetUserJoinedOrganizationsQuery req, 
        CancellationToken cancellationToken)
    {
        var user = await _masterUow.Repository<User>().GetByIdAsync(req.UserId);

        if (user is null)
        {
            return ResponseResult<OrganizationDto[]>.CreateError($"Could not find an user with ID '{req.UserId}'");
        }

        var tenantsIds = user.GetJoinedTenantIds();
        var organizations = await _masterUow.Repository<Tenant>().GetListAsync(i => tenantsIds.Contains(i.Id));

        var organizationsDto = organizations
            .Select(i => new OrganizationDto
            {
                TenantId = i.Id,
                Name = i.Name,
                DisplayName = i.CompanyName!
            })
            .ToArray();
        
        return ResponseResult<OrganizationDto[]>.CreateSuccess(organizationsDto);
    }
}
