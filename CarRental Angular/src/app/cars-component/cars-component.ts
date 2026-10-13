import { asNativeElements, ChangeDetectorRef, Component,OnInit, Query, StreamingResourceOptions } from '@angular/core';
import { CarsHttpService } from '../../services/cars-http-service';
import { filter, finalize, Observable, tap } from 'rxjs';
import { AsyncPipe, JsonPipe, NgClass } from '@angular/common';
import { Car } from '../../entities/models/car';
import { CarsQuantityGroupedByBrand } from '../../entities/models/carsQuantityGroupedByBrand';
import { CarsQuantityGroupedByBrandVM } from '../../entities/view-models/carsQuantityGroupedByBrandVM';
import { CarsQuantityGroupedByCarTypeVM } from '../../entities/view-models/carsQuantityGroupedByCarTypeVM';
import { CarsQuantityGroupedByVM } from '../../entities/view-models/carsQuantityGroupedByVM';
import { CarsQuantityGroupedByCarEngineTypeVM } from '../../entities/view-models/carsQuantityGroupedByCarEngineTypeVM';
import { CarsQuantityGroupedByDateOfProductionVM } from '../../entities/view-models/CarsQuantityGroupedByDateOfProductionVM';
import { FormsModule, NgModel } from '@angular/forms';

export enum filterTypes {
	CarBrand = "CarBrand",
	CarType = "CarType",
	CarEngineType = "CarEngineType",
	YearOfProduction = "YearOfProduction"
}

@Component({
  selector: 'app-cars-component',
  imports: [NgClass, FormsModule],
  templateUrl: './cars-component.html',
  styleUrl: './cars-component.css',
})
export class CarsComponent {
	public dropDownsStates: Map<filterTypes, boolean> = new Map<filterTypes, boolean>
	([
		[filterTypes.CarBrand, false],
		[filterTypes.CarType, false],
		[filterTypes.CarEngineType, false],
		[filterTypes.YearOfProduction, false],
	]);

	public filterTypes = filterTypes;
	public availableCars : Car[] = [];
	public carsQuantityGroupedByBrandsVM: CarsQuantityGroupedByBrandVM[] = [];
	public carsQuantityGroupedByCarTypesVM: CarsQuantityGroupedByCarTypeVM[] = [];
	public carsQuantityGroupedByCarEngineTypesVM: CarsQuantityGroupedByCarEngineTypeVM[] = [];
	public carsQuantityGroupedByDatesOfProductionVM: CarsQuantityGroupedByDateOfProductionVM[] = [];
	public queries : Set<string> = new Set<string>();
	private currentFilterType : filterTypes | null = null;
	public searched : string = "";
	constructor (private carsHttpService: CarsHttpService, private cdr: ChangeDetectorRef) {
		this.getData();
	}
	
	ngOnInit() {
	}

	private getData () : void {
		this.getAllAvailableCars("");
		this.getCarsQuantityGroupedByBrands("");
		this.getCarsQuantityGroupedByCarTypes("");
		this.getCarsQuantityGroupedByCarEngineTypes("");
		this.getCarsQuantityGroupedByDateOfProduction("");
	}

	public getFilteredData() : void{
		const query : string = this.concatQueries(this.queries);

		if(query.length === 0)
		{
			return;
		}

		this.getAllAvailableCars(query);
		this.getCarsQuantityGroupedByBrands(query);
		this.getCarsQuantityGroupedByCarTypes(query);
		this.getCarsQuantityGroupedByCarEngineTypes(query);
		this.getCarsQuantityGroupedByDateOfProduction(query);
	}

	public toggleDropDown(key:filterTypes): void {
		if(!this.dropDownsStates.has(key))
		{
			return;
		}

		this.dropDownsStates.set(key, !this.dropDownsStates.get(key));
	}

	public toggleListElement(arr: CarsQuantityGroupedByVM[], id: number, value: string | number, filterType: filterTypes) : void {
		arr.filter(obj => obj.id === id).forEach(obj => {
			obj.isChosen = !obj.isChosen;
			
			const query : string = this.makeQueryString(obj.queryKey, value);
			if(obj.isChosen === false) {
				if(this.queries.has(query)){
					this.queries.delete(query);
				}
			}
			else{
				if(!this.queries.has(query)){
					this.queries.add(query);
				}
			}
		});

		this.currentFilterType = filterType;
	}
	
	public getAllAvailableCars(query: string) : void {
		this.carsHttpService.getAllAvailableCars(query).subscribe(data => {
			this.availableCars = data;
			this.cdr.markForCheck();
		});
		
	}

	public getCarsQuantityGroupedByBrands(query: string):void {
		this.carsHttpService.getCarsQuantityGroupedByBrand(query).subscribe(data => {
			if(this.carsQuantityGroupedByBrandsVM.length === 0)
			{
				this.carsQuantityGroupedByBrandsVM = [];
				data.forEach(value => this.carsQuantityGroupedByBrandsVM.push({...value, isChosen : false, queryKey:"car_brand"}));
			}
			else
			{
				if(this.currentFilterType !== filterTypes.CarBrand) {
					this.carsQuantityGroupedByBrandsVM = this.carsQuantityGroupedByBrandsVM.filter(obj => obj.isChosen == true);
					
					if(this.carsQuantityGroupedByBrandsVM.length < data.length) {
						for(let i = 0; i < data.length; i++) {
							let index : number = this.carsQuantityGroupedByBrandsVM.findIndex(x => x.brand === data[i].brand);
							if(index !== -1) {
								this.carsQuantityGroupedByBrandsVM[index].carsQuantity = data[i].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByBrandsVM.push({...data[i], isChosen: false, queryKey:"car_brand"});
							}
						}
					}
					else {
						for(let i = 0; i < this.carsQuantityGroupedByBrandsVM.length; i++) {
							let index : number = data.findIndex(x => x.brand === this.carsQuantityGroupedByBrandsVM[i].brand);
							if(index !== -1) {
								this.carsQuantityGroupedByBrandsVM[i].carsQuantity = data[index].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByBrandsVM[i].carsQuantity = 0;
								
							}
						}
					}
				}
				else {
					this.carsQuantityGroupedByBrandsVM = this.carsQuantityGroupedByBrandsVM.filter(obj => obj.isChosen == true);
					if(this.carsQuantityGroupedByBrandsVM.length < data.length) {
						for(let i = 0; i < data.length; i++) {
							let index : number = this.carsQuantityGroupedByBrandsVM.findIndex(x => x.brand === data[i].brand);
							if(index !== -1) {
								this.carsQuantityGroupedByBrandsVM[index].carsQuantity = data[i].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByBrandsVM.push({...data[i], isChosen: false, queryKey:"car_brand"});
							}
						}
					}
					else {
						for(let i = 0; i < this.carsQuantityGroupedByBrandsVM.length; i++) {
							let index : number = data.findIndex(x => x.brand === this.carsQuantityGroupedByBrandsVM[i].brand);
							if(index !== -1) {
								this.carsQuantityGroupedByBrandsVM[i].carsQuantity = data[index].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByBrandsVM[i].carsQuantity = 0;
								
							}
						}
					}
				}
			}
			
			this.cdr.markForCheck();
		});
	}

	public getCarsQuantityGroupedByCarTypes(query: string):void {
			this.carsHttpService.getCarsQuantityGroupedByCarTypes(query)
				.pipe(finalize(() => console.log("car types are loaded")))
				.subscribe(data => {
				if(this.carsQuantityGroupedByCarTypesVM.length === 0) {
					data.forEach(value => this.carsQuantityGroupedByCarTypesVM.push({...value,isChosen : false, queryKey:"car_type"}))
				} 
				else
				{
					if(this.currentFilterType !== filterTypes.CarType) {
						this.carsQuantityGroupedByCarTypesVM = this.carsQuantityGroupedByCarTypesVM.filter(obj => obj.isChosen == true);
						
						if(this.carsQuantityGroupedByCarTypesVM.length < data.length) {
							for(let i = 0; i < data.length; i++) {
								let index : number = this.carsQuantityGroupedByCarTypesVM.findIndex(x => x.carType === data[i].carType);
								if(index !== -1) {
									this.carsQuantityGroupedByCarTypesVM[index].carsQuantity = data[i].carsQuantity;
								}
								else {
									this.carsQuantityGroupedByCarTypesVM.push({...data[i], isChosen: false, queryKey:"car_type"});
								}
							}
						}
						else {
							for(let i = 0; i < this.carsQuantityGroupedByCarTypesVM.length; i++) {
								let index : number = data.findIndex(x => x.carType === this.carsQuantityGroupedByCarTypesVM[i].carType);
								if(index !== -1) {
									this.carsQuantityGroupedByCarTypesVM[i].carsQuantity = data[index].carsQuantity;
								}
								else {
									this.carsQuantityGroupedByCarTypesVM[i].carsQuantity = 0;
								}
							}
						}
					}
					else {
						for(let carsQuantityGroupedByCarTypeVM of this.carsQuantityGroupedByCarTypesVM) {
							let index : number = data.findIndex(x => x.carType === carsQuantityGroupedByCarTypeVM.carType);
							
							if(index !== -1)
							{
								carsQuantityGroupedByCarTypeVM.carsQuantity = data[index].carsQuantity;
							}
						}
					}
				}
				this.cdr.markForCheck();
			})
	}

	public getCarsQuantityGroupedByCarEngineTypes(query: string): void {
		this.carsHttpService.getCarsQuantityGroupedByCarEngineTypes(query).subscribe(data => {
			if(this.carsQuantityGroupedByCarEngineTypesVM.length === 0) {
				data.forEach(value => this.carsQuantityGroupedByCarEngineTypesVM.push({...value,isChosen :false, queryKey:"car_engine_type"}))
			} 
			else {
				if(this.currentFilterType !== filterTypes.CarEngineType) {
					this.carsQuantityGroupedByCarEngineTypesVM = this.carsQuantityGroupedByCarEngineTypesVM.filter(obj => obj.isChosen == true);
					
					if(this.carsQuantityGroupedByCarEngineTypesVM.length < data.length) {
						for(let i = 0; i < data.length; i++) {
							let index : number = this.carsQuantityGroupedByCarEngineTypesVM.findIndex(x => x.carEngineType === data[i].carEngineType);
							if(index !== -1) {
								this.carsQuantityGroupedByCarEngineTypesVM[index].carsQuantity = data[i].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByCarEngineTypesVM.push({...data[i], isChosen: false, queryKey:"car_engine_type"});
							}
						}
					}
					else {
						for(let i = 0; i < this.carsQuantityGroupedByCarEngineTypesVM.length; i++) {
							let index : number = data.findIndex(x => x.carEngineType === this.carsQuantityGroupedByCarEngineTypesVM[i].carEngineType);
							if(index !== -1) {
								this.carsQuantityGroupedByCarEngineTypesVM[i].carsQuantity = data[index].carsQuantity;
							}
							else {
								this.carsQuantityGroupedByCarEngineTypesVM[i].carsQuantity = 0;
							}
						}
					}
				}
				else {
					for(let carsQuantityGroupedByCarEngineTypeVM of this.carsQuantityGroupedByCarEngineTypesVM) {
						let index : number = data.findIndex(x => x.carEngineType === carsQuantityGroupedByCarEngineTypeVM.carEngineType);
						
						if(index !== -1)
						{
							carsQuantityGroupedByCarEngineTypeVM.carsQuantity = data[index].carsQuantity;
						}
					}
				}
			}
			
			this.cdr.markForCheck();
		});
	}

	public getCarsQuantityGroupedByDateOfProduction(query: string): void {
			this.carsHttpService.getCarsQuantityGroupedByDateOfProduction(query).subscribe(data => {
				if(this.carsQuantityGroupedByDatesOfProductionVM.length === 0) {
					data.forEach(value => this.carsQuantityGroupedByDatesOfProductionVM.push({...value, isChosen :false, queryKey:"year_of_production"}))
				} else {
					if(this.currentFilterType !== filterTypes.YearOfProduction) {
						this.carsQuantityGroupedByDatesOfProductionVM = this.carsQuantityGroupedByDatesOfProductionVM.filter(obj => obj.isChosen == true);
						
						if(this.carsQuantityGroupedByDatesOfProductionVM.length < data.length) {
							for(let i = 0; i < data.length; i++) {
								let index : number = this.carsQuantityGroupedByDatesOfProductionVM.findIndex(x => x.yearOfProduction === data[i].yearOfProduction);
								if(index !== -1) {
									this.carsQuantityGroupedByDatesOfProductionVM[index].carsQuantity = data[i].carsQuantity;
								}
								else {
									this.carsQuantityGroupedByDatesOfProductionVM.push({...data[i], isChosen: false, queryKey:"year_of_production"});
								}
							}
						}
						else {
							for(let i = 0; i < this.carsQuantityGroupedByDatesOfProductionVM.length; i++) {
								let index : number = data.findIndex(x => x.yearOfProduction === this.carsQuantityGroupedByDatesOfProductionVM[i].yearOfProduction);
								if(index !== -1) {
									this.carsQuantityGroupedByDatesOfProductionVM[i].carsQuantity = data[index].carsQuantity;
								}
								else {
									this.carsQuantityGroupedByDatesOfProductionVM[i].carsQuantity = 0;
								}
							}
						}
					}
					else {
						for(let carsQuantityGroupedByDateOfProductionVM of this.carsQuantityGroupedByDatesOfProductionVM) {
							let index : number = data.findIndex(x => x.yearOfProduction === carsQuantityGroupedByDateOfProductionVM.yearOfProduction);
							
							if(index !== -1)
							{
								carsQuantityGroupedByDateOfProductionVM.carsQuantity = data[index].carsQuantity;
							}
						}
					}
				}

				this.cdr.markForCheck();
			});
	}

	private makeQueryString(key:string, value:string | number) : string {
		let result : string = "";

		if(typeof(value) === "number") {
			result = `${key}=${value}&`;
		}
		else {
			result = `${key}='${value}'&`;
		}
		return result;
	}

	private concatQueries(queriesSet:Set<string>) : string {
		if(queriesSet.size === 0)
		{
			return "";
		}

		let result = "?";

		for(let query of queriesSet) {
			result += query;
		}

		result = result.substring(0, result.length - 1);
		return result;
	}

	public getSearchedCars() : void {
		 
	}
	
}
