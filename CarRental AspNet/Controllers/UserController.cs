using System.Text.Json;
using CarRental.Entities.DTO;
using CarRental.Models.Entities;
using CarRental.Repositories;
using CarRental.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;


[Route("[controller]")]
public class UserController : ControllerBase
{
	
	private readonly IUserRepository userRepository;
	private readonly AuthService authService;
	public UserController(IUserRepository userRepository, AuthService authService)
	{
		this.userRepository = userRepository;
		this.authService = authService;
	}
	
	[HttpGet("all")]
	public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
	{
		List<User> users = new List<User>();
		// if(Request.Query.Count != 0)
		// {
		// 	users = await this.userRepository.GetFiltered(Request.Query.ToDictionary(), cancellationToken);
		// }
		// else
		// {
		// 	users = await this.userRepository.GetAllAsync(cancellationToken);
		// }

		return Ok(users);
	}	

	
	
	
}
