import { CarsQuantityGroupedByVM } from "./carsQuantityGroupedByVM";

export class CarsQuantityGroupedByBrandVM extends CarsQuantityGroupedByVM{
	constructor(id:number, public brand: string, queryKey:string, carsQuantity: number, isChosen: boolean){
		super(id, carsQuantity,isChosen, queryKey);
	}
}
