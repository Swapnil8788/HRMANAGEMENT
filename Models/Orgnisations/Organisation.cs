using System;

namespace HRManagement.Models.Orgnisations;

public class Organisation
{
    public int Id { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
