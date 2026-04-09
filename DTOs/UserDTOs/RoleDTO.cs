using System;
using System.Text.Json.Serialization;

namespace HRManagement.DTOs.UserDTOs;

public class RoleDTO
{
    public int Code {get;set;}

    public string CodeDesc{get;set;} = string.Empty;
}
