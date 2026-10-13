import { CarsQuantityGroupedByVM } from "./carsQuantityGroupedByVM";

export class CarsQuantityGroupedByCarEngineTypeVM extends CarsQuantityGroupedByVM{
	constructor(id:number,  carsQuantity: number, isChosen: boolean, queryKey:string, public carEngineType: string){
		super(id, carsQuantity, isChosen, queryKey);
	}
}