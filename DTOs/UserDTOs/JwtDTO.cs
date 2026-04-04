using System;

namespace HRManagement.DTOs.UserDTOs;

public class JwtDTO
{
    public string Email{get;set;} = string.Empty;
    public List<string> Roles{get;set;} = new List<string>();
}
