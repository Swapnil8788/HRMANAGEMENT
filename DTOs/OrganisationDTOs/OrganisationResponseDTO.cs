using System;

namespace HRManagement.DTOs.OrganisationDTOs;

public class OrganisationResponseDTO
{
    public int Id{get;set;}
    public string OrgName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
