using System;
using System.Security.Cryptography;
using System.Text;
using HRManagement.Models;

namespace HRManagement.Services;

public class RefreshTokenGeneratorService
{
    public string ComputeSha256Hash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
     public string GenerateRefreshToken()
    {
        var randomBytes = new Byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
    public (RefreshToken , string) CreateRefreshToken(int userId, string ipAddress)
    {
        var rawToken = GenerateRefreshToken();
        var hashRawToken = ComputeSha256Hash(rawToken);
        var refreshToken = new RefreshToken
        {
            Token = hashRawToken,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            IsRevoked = false
        };
        return (refreshToken, rawToken);
    }
}
