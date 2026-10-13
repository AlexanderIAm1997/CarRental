namespace CarRental.Entities.DTO;
    public class CarDTO
    {
		public int Id {get;set;}
		public string Brand {get;set;}
		public string Model {get;set;}
		public DateOnly DateOfProduction {get;set;}
		public string EngineType {get;set;}
		public string Description{get;set;}
		public string CarType {get;set;}
		public int Mileage {get;set;}
		public decimal PricePerDay{get;set;}   
    }
