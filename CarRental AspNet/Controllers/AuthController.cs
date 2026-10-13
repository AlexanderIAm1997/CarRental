using System.Collections.ObjectModel;
using System.Net;
using CarRental.Entities.DTO;
using CarRental.Entities.Models;
using CarRental.Models.Entities;
using CarRental.Repositories;
using CarRental.Services;
using CarRental.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;
// [ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly AuthService authService;
	private readonly IValidator<UserDTO> userValidator;
	private readonly IValidator<UserLoginDTO> userLoginDTOValidator;
	private readonly IUserRepository userRepository;
	private readonly IRefreshTokenRepository refreshTokenRepository;
	private readonly TokenValidator tokenValidator;
	public AuthController(AuthService authService, IValidator<UserDTO> userValidator, IValidator<UserLoginDTO> userLoginDTOValidator, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, TokenValidator tokenValidator)
	{
		this.authService = authService;
		this.userValidator = userValidator;
		this.userLoginDTOValidator = userLoginDTOValidator;
		this.userRepository = userRepository;
		this.refreshTokenRepository = refreshTokenRepository;
		this.tokenValidator = tokenValidator;
	}
	
	[HttpPost("registration")]
	public async Task<IActionResult> RegistrateUser([FromBody] UserDTO userDTO, CancellationToken cancellationToken = default)
	{
		Console.WriteLine(userDTO == null);
		var result = await userValidator.ValidateAsync(userDTO);

		if (!result.IsValid)
		{
			//Problem details
			return BadRequest(result.Errors);
		}

		User? user = await this.userRepository.GetByEmailAsync(userDTO.Email, cancellationToken);
		if(user != null)
		{
			//Correct answer
			return Unauthorized("Користувач з логіном " + userDTO.Email + " вже існує");
		}
		
		await this.userRepository.RegisterAsync(userDTO,cancellationToken);
		//Todo redirect back
	// 	 if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
    // 	 {
    //   	return Redirect(returnUrl);
    // 	 } 
		
		return Redirect(Url.Content("http://localhost:5255/car/cars"));
	}

	[HttpPost("authentication")]
	public async Task<IActionResult> AuthenticateUser([FromBody] UserLoginDTO userLoginDTO, CancellationToken cancellationToken = default)
	{
		var result = await this.userLoginDTOValidator.ValidateAsync(userLoginDTO);
		if (!result.IsValid)
		{
			return BadRequest(result.Errors.ToList());
		}

		User? user = await this.userRepository.GetByEmailAsync(userLoginDTO.Login, cancellationToken);
		if(user == null)
		{
			return Unauthorized("Користувач з логіном " + userLoginDTO.Login + " не існує");
		}

		bool isValidPassword = BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.Password);

		if(isValidPassword == false)
		{
			return Unauthorized("Неправильний логін або пароль");
		}

		string accessToken = this.authService.GenerateAccessToken(user);
		string refreshToken = this.authService.GenerateRefreshToken();
		await this.refreshTokenRepository.StoreAsync(refreshToken,user, cancellationToken);

    	return Ok(new
			{
				accessToken = accessToken,
				refreshToken = refreshToken,
			});
	}	

	//if 401
	[HttpPost("refresh")]
	public async Task<IActionResult> CheckAndSendTokens(CancellationToken cancellationToken = default)
	{
		string refreshTokenString = Request.Cookies["refreshToken"];
		
		if(refreshTokenString == null)
		{
			return Redirect("http://localhost:5255/auth/authentication");
		}
		
		bool isRefreshTokenValid = await this.tokenValidator.IsRefreshTokenValid(refreshTokenString, cancellationToken);
	
		if(isRefreshTokenValid == false)
		{
			return Redirect("http://localhost:5255/auth/authentication");
		}

		RefreshToken refreshToken = await this.refreshTokenRepository.GetByToken(refreshTokenString, cancellationToken);
		User user = await this.userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

		if(user == null)
		{
			return Redirect("http://localhost:5255/auth/registration");
		}

		string newAccessToken = this.authService.GenerateAccessToken(user);
		string newRefreshToken = this.authService.GenerateRefreshToken();
		await this.refreshTokenRepository.StoreAsync(newRefreshToken,user, cancellationToken);

    	return Ok(new
			{
				accessToken = newAccessToken,
				refreshToken = newRefreshToken,
			});
	}
}