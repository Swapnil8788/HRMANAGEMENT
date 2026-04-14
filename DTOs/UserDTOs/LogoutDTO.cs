using System;

namespace HRManagement.DTOs.UserDTOs;

public class LogoutDTO
{
    public string refreshToken { get; set; } = string.Empty;
}
