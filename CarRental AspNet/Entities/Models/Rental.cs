namespace CarRental.Entities.Models;

public class Rental
{
	public int Id {get;set;}
	public int UserId{get;set;}
	public int CarId {get;set;}
	public int PaymentId {get;set;}
	public DateTime StartDate {get;set;}
	public DateTime EndDate {get;set;}
	public int Rate {get;set;}
	public int Mileage {get;set;}
}
