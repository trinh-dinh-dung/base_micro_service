using System;

namespace Application.HttpClients.Models;

public record UserDto(
    Guid Id,
    string UserName,
    string FullName,
    string? Email,
    string? PhoneNumber);

public record DepartmentDto(
    Guid Id,
    string DepartmentCode,
    string DepartmentName,
    Guid? ParentId);
