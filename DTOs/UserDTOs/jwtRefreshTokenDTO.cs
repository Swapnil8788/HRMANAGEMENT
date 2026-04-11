using System;

namespace HRManagement.DTOs.UserDTOs;

public class jwtRefreshTokenDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
