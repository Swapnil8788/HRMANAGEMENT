using System;

namespace HRManagement.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public string ReplacedByToken { get; set; } = string.Empty;
}
