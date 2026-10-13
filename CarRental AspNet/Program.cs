using System.Text;
using CarRental.Middleware;
using CarRental.Repositories;
using CarRental.Validators.FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using CarRental.Services;
using System.Security.Claims;
using Scalar.AspNetCore;
using CarRental.Validators;
//add swagger or open api
// routes

var builder = WebApplication.CreateBuilder(args);

DbInitializer dbInitializer = new DbInitializer(builder.Configuration);

async Task InitializeData()
{
	await dbInitializer.CreateDatabase();
	await dbInitializer.CreateTables();
	await dbInitializer.AddConstraints();	
	await dbInitializer.SeedRolesAsync();
	await dbInitializer.SeedCarBrandsAsync();
	await dbInitializer.SeedCarTypesAsync();
	await dbInitializer.SeedCarEngineTypes();	
	await dbInitializer.SeedCars();
}

InitializeData();

builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginDTOValidator>();

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<TokenValidator>();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration.GetSection("Jwt")["Issuer"],
			ValidAudience = builder.Configuration.GetSection("Jwt")["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Key"]))
		};
	});
		
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("DefaultUserPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", (policy) =>
    {
        policy.WithOrigins("http://localhost:4200")
        	.AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	app.MapScalarApiReference(); 
}

// app.UseHttpsRedirection();
app.UseCors("Angular");
app.MapControllers();
// app.UseExceptionHandler();
app.Run();

