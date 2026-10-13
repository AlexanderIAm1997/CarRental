import { CarsQuantityGroupedBy } from "./CarsQuantityGroupedBy";

export class CarsQuantityGroupedByBrand extends CarsQuantityGroupedBy {
	constructor(id:number, carsQuantity:number, public brand: string) {
		super(id, carsQuantity );
	}
}



