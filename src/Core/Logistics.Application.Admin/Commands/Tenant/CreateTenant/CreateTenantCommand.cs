﻿using Logistics.Domain.ValueObjects;
using Logistics.Shared;
using MediatR;

namespace Logistics.Application.Admin.Commands;

public class CreateTenantCommand : IRequest<ResponseResult>
{
    public string Name { get; set; } = default!;
    public string? CompanyName { get; set; }
    public Address? CompanyAddress { get; set; }
}
