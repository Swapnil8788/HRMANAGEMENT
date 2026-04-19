using System;
using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.OrganisationDTOs;

public class OrganisationDTO
{
    [Required]
    public string OrgName { get; set; } = string.Empty;
    [Required]
    public string Location { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
}
