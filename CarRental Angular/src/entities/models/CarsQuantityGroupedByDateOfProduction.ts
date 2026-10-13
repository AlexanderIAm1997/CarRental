import { CarsQuantityGroupedBy } from "./CarsQuantityGroupedBy";

export class CarsQuantityGroupedByDateOfProduction extends CarsQuantityGroupedBy{
	constructor(id:number, carsQuantity:number, public yearOfProduction: number) {
		super(id, carsQuantity);
	}
}