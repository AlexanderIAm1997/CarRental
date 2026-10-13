import { CarsQuantityGroupedBy } from "./CarsQuantityGroupedBy";

export class CarsQuantityGroupedByCarType extends CarsQuantityGroupedBy{
	constructor(id:number, carsQuantity:number, public carType: string) {
		super(id, carsQuantity);
	}
}