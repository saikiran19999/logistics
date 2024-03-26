﻿using Logistics.Client.Models;
using Logistics.Shared;
using Logistics.Shared.Models;

namespace Logistics.Client.Abstractions;

public interface IEmployeeApi
{
    Task<ResponseResult<EmployeeDto>> GetEmployeeAsync(string userId);
    Task<PagedResponseResult<EmployeeDto>> GetEmployeesAsync(SearchableQuery query);
    Task<ResponseResult> CreateEmployeeAsync(CreateEmployee command);
    Task<ResponseResult> UpdateEmployeeAsync(UpdateEmployee command);
    Task<ResponseResult> DeleteEmployeeAsync(string userId);
}
