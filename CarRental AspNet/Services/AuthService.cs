using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CarRental.Entities.DTO;
using CarRental.Entities.Models;
using CarRental.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Services;

public class AuthService
{
	private readonly IConfiguration configuration;
	public AuthService(IConfiguration configuration)
    {
    	this.configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var jwtSettings = this.configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.FirstName),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
            signingCredentials: credentials
		);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

	public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

	public bool IsValidRefreshToken(string refreshToken, RefreshToken refreshTokenObject)
	{
		return true;
	}
}
