using System.Data;
using System.Globalization;
using CarRental.Entities.Models;
using CarRental.Models.Entities;
using Microsoft.Data.SqlClient;

namespace CarRental.Repositories;

public interface IRefreshTokenRepository
{
	Task<RefreshToken?> GetByToken(string refreshTokenString, CancellationToken cancellationToken);
	Task StoreAsync(string refreshTokenString, User user, CancellationToken cancellationToken);
}
public class RefreshTokenRepository : IRefreshTokenRepository
{
	private readonly IConfiguration configuration;
	private object cancellationToken;

	private string connectionString {get;set;}
	public RefreshTokenRepository(IConfiguration configuration)
	{
		this.configuration = configuration;
		this.connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
	}

	public async Task<RefreshToken?> GetByToken(string refreshTokenString, CancellationToken cancellationToken)
	{
		const string query = 
			@"use car_rental_db;
			select refresh_tokens.refresh_token_id, users.user_id, refresh_tokens.refresh_token, refresh_tokens.issued_at, refresh_tokens.expires_at  
			from refresh_tokens
			left join [users] on users.user_id = refresh_tokens.user_id
			where refresh_tokens.refresh_token = @refresh_token_string";
		
		//ADdVariable to role
		RefreshToken refreshToken = new RefreshToken();
		
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.Add("@refresh_token_string", SqlDbType.VarChar).Value = refreshTokenString;	
				await connection.OpenAsync((CancellationToken)cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					if(reader.HasRows == false)
					{
						refreshToken = null;
					}
					while (await reader.ReadAsync(cancellationToken))
					{
						refreshToken.Id = int.Parse(reader["refresh_token_id"].ToString());
						refreshToken.UserId = int.Parse(reader["user_id"].ToString());
						refreshToken.Token = reader["refresh_token"].ToString();
						refreshToken.IssuedAt = DateTime.ParseExact(reader["issued_at"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture);
						refreshToken.ExpiresAt = DateTime.ParseExact(reader["expires_at"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture);
					}
				}
			}
		}	 

		return refreshToken;
	}

	public async Task StoreAsync(string refreshTokenString, User user, CancellationToken cancellationToken)
	{
		await DeleteInvalidRefreshTokens(user, cancellationToken);
		
		const string query = 
			@"use car_rental_db;
			insert into refresh_tokens (user_id, refresh_token, issued_at, expires_at)
			values(@user_id, @refresh_token, @issued_at, @expires_at)"; 
		
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.Add("@user_id", SqlDbType.Int).Value = user.Id;
				command.Parameters.Add("@refresh_token", SqlDbType.VarChar).Value = refreshTokenString;
				command.Parameters.Add("@issued_at", SqlDbType.DateTime2).Value = DateTime.Now;
				command.Parameters.Add("@expires_at", SqlDbType.DateTime2).Value = DateTime.Now.AddDays(7);

				await connection.OpenAsync(cancellationToken);
				await command.ExecuteNonQueryAsync(cancellationToken);
			}
		}	 

		return;
	}

	private async Task DeleteInvalidRefreshTokens(User user, CancellationToken cancellationToken)
	{
		const string query = 
			@"DELETE FROM refresh_tokens WHERE user_id = @user_id"; 
		
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.Add("@user_id", SqlDbType.Int).Value = user.Id;

				await connection.OpenAsync(cancellationToken);
				await command.ExecuteNonQueryAsync(cancellationToken);
			}
		}	 

		return;
	}
}