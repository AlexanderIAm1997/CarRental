import { CarsQuantityGroupedByVM } from "./carsQuantityGroupedByVM";

export class CarsQuantityGroupedByCarTypeVM extends CarsQuantityGroupedByVM{
	constructor(id:number, carsQuantity: number,queryKey:string, isChosen: boolean, public carType: string){
		super(id, carsQuantity, isChosen,queryKey);
	}
}
