import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Car } from '../entities/models/car';
import { Observable } from 'rxjs';
import { CarsQuantityGroupedByBrand } from '../entities/models/carsQuantityGroupedByBrand';
import { CarsQuantityGroupedByCarType } from '../entities/models/carsQuantityGroupedByCarType';
import { CarsQuantityGroupedByCarEngineType } from '../entities/models/carsQuantityGroupedByCarEngineType';
import { CarsQuantityGroupedByDateOfProduction } from '../entities/models/CarsQuantityGroupedByDateOfProduction';

@Injectable({
  providedIn: 'root',
})
export class CarsHttpService {
	private url: string = "http://localhost:5255/car/cars";

	constructor(private httpClient: HttpClient){}

	public getAllAvailableCars(query:string) : Observable<Car[]> {
		return this.httpClient.get<Car[]>(this.url + query);
	}

	public getCarsQuantityGroupedByBrand(query: string) : Observable<CarsQuantityGroupedByBrand[]> {
		return this.httpClient.get<CarsQuantityGroupedByBrand[]>(this.url + "/car_quantity_grouped_by_brands" + query);
	}

	public getCarsQuantityGroupedByCarTypes(query: string) : Observable<CarsQuantityGroupedByCarType[]> {
		return this.httpClient.get<CarsQuantityGroupedByCarType[]>(this.url + "/car_quantity_grouped_by_car_type" + query);
	}

	public getCarsQuantityGroupedByCarEngineTypes(query: string) : Observable<CarsQuantityGroupedByCarEngineType[]> {
		return this.httpClient.get<CarsQuantityGroupedByCarEngineType[]>(this.url + "/car_quantity_grouped_by_car_engine_type"+ query);
	}

	public getCarsQuantityGroupedByDateOfProduction(query: string) : Observable<CarsQuantityGroupedByDateOfProduction[]> {
		return this.httpClient.get<CarsQuantityGroupedByDateOfProduction[]>(this.url + "/car_quantity_grouped_by_date_of_production" + query);
	}

	public getSearchedCars(query: string) : Observable<Car[]> {
		return this.httpClient.get<Car[]>(this.url + "/search" + query);
	}
}
