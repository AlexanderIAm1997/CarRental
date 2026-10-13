export class Car {
	constructor(
		public car_id: number, 
		public brand: string, 
		public model: string,
		public carType: string, 
		public engineType: string, 
		public dateOfProduction: number,
		public description: string, 
		public pricePerDay: number
	) {}
}