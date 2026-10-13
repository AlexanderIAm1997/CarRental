namespace CarRental.Entities.Models;

public class Car
{
	public int Id {get;set;}
	public string Brand {get;set;}
	public string Model {get;set;}
	public DateOnly DateOfProduction {get;set;}
	public string EngineType {get;set;}
	public string DescriptionUkr{get;set;}
	public string DescriptionEng{get;set;}
	public string CarType {get;set;}
	public int Mileage {get;set;}
	public bool IsAvaliable{get;set;}
	public decimal PricePerDay{get;set;}
}
