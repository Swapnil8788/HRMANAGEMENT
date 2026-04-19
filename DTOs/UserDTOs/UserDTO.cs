using System;

namespace HRManagement.DTOs.UserDTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public int Age { get; set; }
    // public ICollection<string> Roles { get; set; } = new List<string>();
}
