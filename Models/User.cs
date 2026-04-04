using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HRManagement.Models;

public class User
{
    public int UserId{get;set;}
    public string Name { get; set; } = string.Empty;
    public string Email{get;set;}=string.Empty;
    public string PhoneNo{get;set;}=string.Empty;
    //should change to its native type
    public  string PasswordHash{get;set;}=string.Empty;
    public int Age{get;set;}
    public DateTime CreatedAt{get;set;}
    public DateTime ModifiedAt{get;set;}
    public ICollection<UserRole> UserRoles{get;set;} = new List<UserRole>();
}
