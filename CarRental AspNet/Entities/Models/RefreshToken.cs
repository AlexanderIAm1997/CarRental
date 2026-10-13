namespace CarRental.Entities.Models;

public class RefreshToken
{
	public int Id{get;set;}
	public int UserId {get;set;}
	public string Token {get;set;}
	public DateTime IssuedAt {get;set;}
	public DateTime ExpiresAt {get;set;}

}