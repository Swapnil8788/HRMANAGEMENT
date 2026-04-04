using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.UserDTOs;

public class RegistrationUser
{
    [Required]
    public string Name{get;set;}=string.Empty;
    [Required]
    [MinLength(6)]
    public string Email{get;set;}=string.Empty;
    [Required]
    [MinLength(6)]
    public string Password{get;set;}=string.Empty;
    [Required]
    [Compare("Password", ErrorMessage = "Password do not Match")]
    public string Confirm{get;set;}=string.Empty;
    [Required]
    public string PhoneNo{get;set;}=string.Empty;
    [Required]
    public int Age{get;set;}
    [Required]
    [MinLength(1)]
    public ICollection<string> Roles{get;set;} = new List<string>();
}
