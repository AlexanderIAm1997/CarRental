import { CarsQuantityGroupedByVM } from "./carsQuantityGroupedByVM";

export class CarsQuantityGroupedByDateOfProductionVM extends CarsQuantityGroupedByVM{
	constructor( id:number, carsQuantity: number, queryKey:string, isChosen: boolean, public yearOfProduction: number){
		super( id, carsQuantity, isChosen, queryKey);
	}
}