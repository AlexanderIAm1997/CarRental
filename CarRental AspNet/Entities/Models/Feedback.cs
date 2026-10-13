namespace CarRental.Entities.Models;

public class Feedback
{
	public int Id {get;set;}
	public int UserId{get;set;}
	public int CarId {get;set;}
	public string Content {get;set;}
	public DateTime CreatedAt {get;set;}
}
