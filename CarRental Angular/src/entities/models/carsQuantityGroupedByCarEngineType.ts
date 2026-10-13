import { CarsQuantityGroupedBy } from "./CarsQuantityGroupedBy";

export class CarsQuantityGroupedByCarEngineType extends CarsQuantityGroupedBy{
	constructor(id:number, carsQuantity:number, public carEngineType: string) {
		super(id, carsQuantity);
	}
}