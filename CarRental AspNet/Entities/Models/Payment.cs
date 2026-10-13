namespace CarRental.Entities.Models;

public class Payment
{
	public int Id {get;set;}
	public int UserId{get;set;}
	public int CarId {get;set;}
	public int Amount {get;set;}
	public bool Status {get;set;} 
	public DateTime PaidAt {get;set;}
}
