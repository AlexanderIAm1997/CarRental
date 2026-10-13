using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Entities.DTO;

public class UserDTO
{
	public string FirstName {get;set;}
	public string LastName {get;set;}
	public int Age {get;set;}
	public string Email {get;set;}
	public string Phone {get;set;}
	public string Password {get;set;}
}