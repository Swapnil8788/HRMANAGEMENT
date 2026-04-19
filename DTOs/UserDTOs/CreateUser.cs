using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.UserDTOs;

public class CreateUser
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public int Age { get; set; }
    public ICollection<string> Roles { get; set; } = new List<string>();
}
