
using System.Data;
using System.Globalization;
using CarRental.Entities.DTO;
using CarRental.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;

namespace CarRental.Repositories;
public interface IUserRepository
{
	Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
	Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
	Task RegisterAsync(UserDTO userDto, CancellationToken token);
	Task<List<User>> GetFiltered(Dictionary<string, Microsoft.Extensions.Primitives.StringValues> parameters, CancellationToken cancellationToken);
	Task<User?> GetByEmailAsync(string login, CancellationToken cancellationToken);
}

public class UserRepository : IUserRepository
{
	private readonly IConfiguration configuration;
	private string connectionString {get;set;}
	private Dictionary<string, string> QueryKeys {get;set;}
	public UserRepository(IConfiguration configuration)
	{
		this.connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
	}

	public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
	{
		const string query = 
			@"use car_rental_db;
			select users.user_id , users.first_name, users.last_name, users.age, users.email, users.phone, users.hashed_password , users.created_at, roles.role
			from users
			left join roles on roles.role_id = users.role_id";
//ADdVariable to role
		List<User> users = new List<User>();

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{	
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						User user = new User()
						{
							Id = int.Parse(reader["user_id"].ToString()),
							FirstName = reader["first_name"].ToString(),
							LastName = reader["last_name"].ToString(),
							Age = int.Parse(reader["age"].ToString()),
							Email = reader["email"].ToString(),
							Phone = reader["phone"].ToString(),
							Password = reader["hashed_password"].ToString(),
							CreatedAt = DateTime.ParseExact(reader["created_at"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture),
							Role = reader["role"].ToString(),
						};
					}
				}
			}
		}	 

		return users;
	}

	public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		const string query = 
			@"use car_rental_db;
			select users.user_id , users.first_name, users.last_name, users.age, users.email, users.phone, users.hashed_password , users.created_at, roles.role
			from users
			left join roles on roles.role_id = users.role_id
			where users.user_id = @user_id";
			//ADdVariable to role

		User user = new User();
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.Add("@user_id", SqlDbType.Int).Value = id;	
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					if(!reader.HasRows)
					{
						user = null;
					}
					while (await reader.ReadAsync(cancellationToken))
					{
						user.Id = int.Parse(reader["user_id"].ToString());
						user.FirstName = reader["first_name"].ToString();
						user.LastName = reader["last_name"].ToString();
						user.Age = int.Parse(reader["age"].ToString());
						user.Email = reader["email"].ToString();
						user.Phone = reader["phone"].ToString();
						user.Password = reader["hashed_password"].ToString();
						user.CreatedAt = DateTime.ParseExact(reader["created_at"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture);
						user.Role = reader["role"].ToString();
					}
				}
			}
		}	 

		return user;
	}

	public async Task RegisterAsync(UserDTO userDTo, CancellationToken cancellationToken)
	{
		const string query = @"
			INSERT INTO users (first_name, last_name, age, email, phone, hashed_password, created_at, role_id)
			VALUES (@first_name, @last_name, @age, @email, @phone, @hashed_password, @created_at, @role_id);";
		string hashed_password = BCrypt.Net.BCrypt.HashPassword(userDTo.Password);
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.Add("@first_name", SqlDbType.NVarChar).Value = userDTo.FirstName;
				command.Parameters.Add("@last_name", SqlDbType.NVarChar).Value = userDTo.LastName;
				command.Parameters.Add("@age", SqlDbType.Int).Value = userDTo.Age;
				command.Parameters.Add("@email", SqlDbType.NVarChar).Value = userDTo.Email;
				command.Parameters.Add("@phone", SqlDbType.NVarChar).Value = userDTo.Phone;
				command.Parameters.Add("@hashed_password", SqlDbType.NVarChar).Value = hashed_password;
				command.Parameters.Add("@created_at", SqlDbType.DateTime2).Value = DateTime.Now;
				command.Parameters.Add("@role_id", SqlDbType.Int).Value = 2;

				await connection.OpenAsync(cancellationToken);
				await command.ExecuteNonQueryAsync(cancellationToken);
			}	
		}
	}

	public async Task<List<User>> GetFiltered(Dictionary<string, StringValues> parameters, CancellationToken cancellationToken)
	{
		return new List<User>();
	}

	// public async Task<bool> IsUserExistAsync(UserLoginDTO userLoginDTO, CancellationToken cancellationToken)
	// {
	// 	const string query = @"SELECT Count(id) FROM users where users.email = @email and users.password = @password";

	// 	bool isUserExist = false;

	// 	using (var connection = new SqlConnection(this.connectionString))
	// 	{
	// 		using (var command = new SqlCommand(query, connection))
	// 		{	
	// 			await connection.OpenAsync(cancellationToken);

	// 			using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
	// 			{
	// 				command.Parameters.Add("@email", SqlDbType.NVarChar).Value = userLoginDTO.Login;
	// 				command.Parameters.Add("@password", SqlDbType.NVarChar).Value = userLoginDTO.Password;
					
	// 				while (await reader.ReadAsync(cancellationToken))
	// 				{
	// 					isUserExist = true;
	// 				}
	// 			}
	// 		}
	// 	}

	// 	return isUserExist;	 
	// }

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
	{
		const string query = $@"select users.user_id , users.first_name, users.last_name, users.age, users.email, users.phone, users.hashed_password, users.created_at, roles.role_id, roles.role
			from users
			left join roles on roles.role_id = users.role_id
			where users.email = @email";

		User user = new User();

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{	
				command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					if(!reader.HasRows)
					{
						user = null;
					}

					while (await reader.ReadAsync(cancellationToken))
					{
						user.Id = int.Parse(reader["user_id"].ToString());
						user.FirstName = reader["first_name"].ToString();
						user.LastName = reader["last_name"].ToString();
						user.Age = int.Parse(reader["age"].ToString());
						user.Email = reader["email"].ToString();
						user.Phone = reader["phone"].ToString();
						user.Password = reader["hashed_password"].ToString();
						user.CreatedAt = DateTime.ParseExact(reader["created_at"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture);
						user.Role = reader["role"].ToString();
					}
				}
			}
		}

		return user;
	}
}