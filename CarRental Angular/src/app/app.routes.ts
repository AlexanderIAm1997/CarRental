import { Routes } from '@angular/router';
import { HomeComponent } from './home-component/home-component';
import { CarsComponent } from './cars-component/cars-component';
import { CarRentComponent } from './car-rent-component/car-rent-component';

export const routes: Routes = 
[
	{"path" : "home", component : HomeComponent},
	{"path" : "cars", component : CarsComponent},
	{"path" : "cars/:id", component: CarRentComponent}
	// {"path" : "/contacts", component : HomeComponent},
	// {"path" : "/about_us", component : HomeComponent},
	// {"path" : "/feedbacks", component : HomeComponent},
];

