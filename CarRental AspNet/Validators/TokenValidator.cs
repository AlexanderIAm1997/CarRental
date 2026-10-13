using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CarRental.Entities.Models;
using CarRental.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CarRental.Validators;

public class TokenValidator
{
	private readonly IConfiguration configuration;
	private readonly IRefreshTokenRepository refreshTokenRepository;

	public TokenValidator(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
	{
		this.configuration = configuration;
		this.refreshTokenRepository = refreshTokenRepository;
	}
	public bool IsAccessTokenValid(string accessToken)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var validationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
			ValidateIssuer = true,
			ValidIssuer = configuration["Jwt:Issuer"],
			ValidateAudience = true,
			ValidAudience = configuration["Jwt:Audience"],
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};

		try
		{
			var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
		}
		catch
		{
			
			return false;
		} 

		return true;
	}

	public async Task<bool> IsRefreshTokenValid(string refreshTokenString, CancellationToken cancellationToken = default)
	{
		RefreshToken refreshToken = await this.refreshTokenRepository.GetByToken(refreshTokenString, cancellationToken);
		
		if(refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
		{
			return false;	
		}

		return true;
	}
}