using CarRental.Entities.DTO;
using CarRental.Entities.Models;
using CarRental.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CarRental.Controllers;

    // [ApiController]
	//Todo : try this with exception middleware!!!
[Route("[controller]")]
public class CarController : ControllerBase
{
	private readonly ICarRepository carRepository;

	public CarController(ICarRepository carRepository)
	{
		this.carRepository = carRepository;
	}

	// [Authorize(Policy = "DefaultUserPolicy")]
	[HttpGet("cars")]
	public async Task<IActionResult> GetAvailableCarsAsync(CancellationToken cancellationToken)
	{	
		List<CarDTO> cars = new List<CarDTO>();
		if(Request.Query.Count != 0)
		{
			cars = await this.carRepository.GetFilteredAsync(Request.Query.ToDictionary() ,cancellationToken);
		}
		else
		{ 
			Console.WriteLine(DateTime.Now);
			cars = await this.carRepository.GetAvailableAsync(cancellationToken);
		}
		return Ok(cars);
	}

	[HttpGet("cars/{id:int}")]
	public async Task<IActionResult> GetCarById(int id, CancellationToken cancellationToken)
	{	
		CarDTO carDto = await this.carRepository.GetByIdAsync(id, cancellationToken);	
		return Ok(carDto);
	}
	
	// [HttpGet("cars/search")]
	// public async Task<IActionResult> GetCarBySearch([FromQuery] string queryString)
	// {

	// }

	[HttpGet("cars/car_quantity_grouped_by_brands")]
	public async Task<IActionResult> GetCarQuantityGroupedByBrands(CancellationToken cancellationToken = default)
	{
		List<CarsQuantityGroupedByBrandDTO> carsQuantityGroupedByBrands = await this.carRepository.GetCarsQuantityGroupedByBrandsAsync(Request.Query.ToDictionary(),cancellationToken);
		return Ok(carsQuantityGroupedByBrands);
	}

	[HttpGet("cars/car_quantity_grouped_by_car_type")]
	public async Task<IActionResult> GetCarQuantityGroupedByCarType(CancellationToken cancellationToken = default)
	{
		List<CarsQuantityGroupedByCarTypeDTO> carsQuantityGroupedByCarTypes = await this.carRepository.GetCarsQuantityGroupedByCarTypesAsync(Request.Query.ToDictionary(), cancellationToken);
		return Ok(carsQuantityGroupedByCarTypes);
	}

	[HttpGet("cars/car_quantity_grouped_by_car_engine_type")]
	public async Task<IActionResult> GetCarsQuantityGroupedByCarEngineType(CancellationToken cancellationToken = default)
	{
		List<CarsQuantityGroupedByCarEngineTypeDTO> carsQuantityGroupedByCarEngineTypes = await this.carRepository.GetCarsQuantityGroupedByCarEngineTypesAsync(Request.Query.ToDictionary(),cancellationToken);
		return Ok(carsQuantityGroupedByCarEngineTypes);
	}

	[HttpGet("cars/car_quantity_grouped_by_date_of_production")]
	public async Task<IActionResult> GetCarsQuantityGroupedByDateOfProduction(CancellationToken cancellationToken = default)
	{
		List<CarsQuantityGroupedByYearOfProductionDTO> carsQuantityGroupedByYearOfProduction = await this.carRepository.GetCarsQuantityGroupedByDateOfProductionAsync(Request.Query.ToDictionary(), cancellationToken);
		return Ok(carsQuantityGroupedByYearOfProduction);
	}

	
}
