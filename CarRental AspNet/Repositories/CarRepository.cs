using System.Data;
using System.Globalization;
using CarRental.Entities.DTO;
using CarRental.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;

namespace CarRental.Repositories;

public interface ICarRepository
{
	Task<CarDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
	Task<List<CarDTO>> GetAllAsync(CancellationToken cancellationToken);
	Task<List<CarDTO>> GetAvailableAsync(CancellationToken cancellationToken);
	Task<List<CarDTO>> GetFilteredAsync(Dictionary<string, StringValues> parameters, CancellationToken cancellationToken);
	Task DeleteByIdAsync(int id);
	Task UpdateAsync(Car car);
	Task<List<CarsQuantityGroupedByBrandDTO>> GetCarsQuantityGroupedByBrandsAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken);
	Task<List<CarsQuantityGroupedByCarTypeDTO>> GetCarsQuantityGroupedByCarTypesAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken);
	Task<List<CarsQuantityGroupedByCarEngineTypeDTO>> GetCarsQuantityGroupedByCarEngineTypesAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken);
	Task<List<CarsQuantityGroupedByYearOfProductionDTO>> GetCarsQuantityGroupedByDateOfProductionAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken);
} 

public class CarRepository : ICarRepository
{
	private readonly IConfiguration configuration;
	private string connectionString {get;set;}
	private Dictionary<string, string> QueryKeys {get;set;}
	public CarRepository(IConfiguration configuration)
	{
		this.connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		this.QueryKeys = new Dictionary<string, string>()
		{
			["car_brand"] = "car_brands.car_brand",
			["car_engine_type"] = "car_engine_types.car_engine_type_ukr",
			["car_type"] = "car_types.car_type_ukr",
			["year_of_production"] = "cars.date_of_production",
			["price_per_day_min"] = "cars.price_per_day",
			["price_per_day_max"] = "cars.price_per_day",
		};
	}

	public async Task<List<CarDTO>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		List<CarDTO> cars = new List<CarDTO>();
		const string query = @"use car_rental_db;
			select cars.car_id, car_brands.car_brand, cars.model, car_engine_types.car_engine_type_ukr, cars.date_of_production, cars.description_ukr, car_types.car_type_ukr, cars.mileage, cars.is_available, cars.price_per_day
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarDTO car = new CarDTO()
						{
							Id = int.Parse(reader["car_id"].ToString()),
							Brand = reader["car_brand"].ToString(),
							Model = reader["model"].ToString(),
							DateOfProduction = DateOnly.ParseExact(reader["date_of_production"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture),
							EngineType = reader["car_engine_type_ukr"].ToString(),
							Description = reader["description_ukr"].ToString(),
							CarType = reader["car_type_ukr"].ToString(),
							Mileage = int.Parse(reader["mileage"].ToString()),
							PricePerDay = decimal.Parse(reader["price_per_day"].ToString()),
						};

						cars.Add(car);
					}
				}
			}
		}

		return cars;
	}

	public async Task<List<CarDTO>> GetAvailableAsync(CancellationToken cancellationToken = default)
	{
		const string query = $@"use car_rental_db;
		  							select cars.car_id, car_brands.car_brand, cars.model, car_engine_types.car_engine_type_ukr, cars.date_of_production, cars.description_ukr, car_types.car_type_ukr, cars.mileage, cars.is_available, cars.price_per_day
									from cars
									left join car_brands on cars.car_brand_id = car_brands.car_brand_id
									left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
									left join car_types on cars.car_type_id = car_types.car_type_id
									where cars.is_available = @IsAvailable;";
			
		List<CarDTO> cars = new List<CarDTO>();

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{	
				command.Parameters.Add("@IsAvailable", SqlDbType.Int).Value = 1;
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarDTO carDto = new CarDTO()
						{
							Id = int.Parse(reader["car_id"].ToString()),
							Brand = reader["car_brand"].ToString(),
							Model = reader["model"].ToString(),
							DateOfProduction = DateOnly.ParseExact(reader["date_of_production"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture),
							EngineType = reader["car_engine_type_ukr"].ToString(),
							Description = reader["description_ukr"].ToString(),
							CarType = reader["car_type_ukr"].ToString(),
							Mileage = int.Parse(reader["mileage"].ToString()),
							PricePerDay = decimal.Parse(reader["price_per_day"].ToString()),
						};

						cars.Add(carDto);
					}
				}
			}
		}	 

		return cars; 
	}
	public async Task<CarDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		const string query = $@"use car_rental_db;
			select cars.car_id, car_brands.car_brand, cars.model, car_engine_types.car_engine_type_ukr, cars.date_of_production, cars.description_ukr, car_types.car_type_ukr, cars.mileage, cars.is_available, cars.price_per_day
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id
			where cars.car_id = @Id;";
		
		CarDTO carDto = new CarDTO();

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{	
				command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						carDto.Id = int.Parse(reader["car_id"].ToString());
						carDto.Brand = reader["car_brand"].ToString();
						carDto.Model = reader["model"].ToString();
						carDto.DateOfProduction = DateOnly.Parse(reader["date_of_production"].ToString());
						carDto.EngineType = reader["car_engine_type_ukr"].ToString();
						carDto.Description = reader["description_ukr"].ToString();
						carDto.CarType = reader["car_type_ukr"].ToString();
						carDto.Mileage = int.Parse(reader["mileage"].ToString());
						carDto.PricePerDay = decimal.Parse(reader["price_per_day"].ToString());
					}
				}
			}
		}	 

		return carDto; 
	}
	
	public async Task<List<CarDTO>> GetFilteredAsync( Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken = default)
	{
		List<CarDTO> cars = new List<CarDTO>();
		string query = @"use car_rental_db;
			select cars.car_id, car_brands.car_brand, cars.model, car_engine_types.car_engine_type_ukr, cars.date_of_production, cars.description_ukr, car_types.car_type_ukr, cars.mileage, cars.is_available, cars.price_per_day
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";

		query += CreateConditionString(parametersMap);

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarDTO carDto = new CarDTO()
						{
							Id = int.Parse(reader["car_id"].ToString()),
							Brand = reader["car_brand"].ToString(),
							Model = reader["model"].ToString(),
							DateOfProduction = DateOnly.ParseExact(reader["date_of_production"].ToString(), new string[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss" }, CultureInfo.InvariantCulture),
							EngineType = reader["car_engine_type_ukr"].ToString(),
							Description = reader["description_ukr"].ToString(),
							CarType = reader["car_type_ukr"].ToString(),
							Mileage = int.Parse(reader["mileage"].ToString()),
							PricePerDay = decimal.Parse(reader["price_per_day"].ToString()),
						};

						cars.Add(carDto);
					}
				}
			}
		}

		return cars;
	}
	private string CreateConditionString(Dictionary<string, StringValues> parametersMap)
	{
		if(parametersMap.Count() == 0)
		{
			return "";
		}

		string conditions = " WHERE ";

		foreach (KeyValuePair<string, StringValues> kvp in parametersMap)
		{
			if (!this.QueryKeys.ContainsKey(kvp.Key))
			{
				throw new ArgumentException("Query string is not valid");
			}

			List<string> values = kvp.Value.ToString().Split(",").ToList();
			conditions += "(";
			for(int i = 0; i < values.Count ; i++)
			{
				if (kvp.Key.Equals("price_per_day_min"))
				{
					conditions += $"{this.QueryKeys[kvp.Key]} >= {values[i]} AND ";
				}
				else if (kvp.Key.Equals("price_per_day_max"))
				{
					conditions += $"{this.QueryKeys[kvp.Key]} <= {values[i]} AND ";
				}
				else if(kvp.Key.Equals("year_of_production"))
				{
					if(i < values.Count - 1)
					{
						conditions += $"YEAR({this.QueryKeys[kvp.Key]})={values[i]} OR ";
					}
					else
					{
						conditions += $"YEAR({this.QueryKeys[kvp.Key]})={values[i]}) AND ";
					}
				}
				else if(kvp.Key.Equals("car_brand"))
				{
					
					if(i < values.Count - 1)
					{
						conditions += $"{this.QueryKeys[kvp.Key]} = N{values[i]} OR ";
					}
					else
					{
						conditions += $"{this.QueryKeys[kvp.Key]} = N{values[i]}) AND ";
					}	
				}
				else
				{
					if(i < values.Count - 1)
					{
						conditions += $"{this.QueryKeys[kvp.Key]} = N{values[i]} OR ";
					}
					else
					{
						conditions += $"{this.QueryKeys[kvp.Key]} = N{values[i]}) AND ";
					}
				}
			}	
		}
		
		Console.WriteLine(conditions);
		conditions = conditions.Substring(0, conditions.Length - 5);
		return conditions;
	}

	public Task DeleteByIdAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(Car car)
	{
		throw new NotImplementedException();
	}

	public async Task<List<CarsQuantityGroupedByBrandDTO>> GetCarsQuantityGroupedByBrandsAsync(Dictionary<string, StringValues> parametersMap,CancellationToken cancellationToken)
	{
		List<CarsQuantityGroupedByBrandDTO> carsQuantityGroupedByBrands = new List<CarsQuantityGroupedByBrandDTO>();
		string query = @"use car_rental_db;
			select	ROW_NUMBER() OVER (order by (select null)) AS id, car_brands.car_brand, Count(cars.car_id) as car_quantity
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";
		
		query += this.CreateConditionString(parametersMap);
		query += " group by car_brands.car_brand";

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarsQuantityGroupedByBrandDTO carsQuantityGroupedByBrandDTO = new CarsQuantityGroupedByBrandDTO()
						{
							Id = int.Parse(reader["id"].ToString()),
							Brand = reader["car_brand"].ToString(),
							CarsQuantity = int.Parse(reader["car_quantity"].ToString()),
						};

						carsQuantityGroupedByBrands.Add(carsQuantityGroupedByBrandDTO);
					}
				}
			}
		}

		return carsQuantityGroupedByBrands;
	}

	public async Task<List<CarsQuantityGroupedByCarTypeDTO>> GetCarsQuantityGroupedByCarTypesAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken)
	{
		List<CarsQuantityGroupedByCarTypeDTO> carsQuantityGroupedByCarTypes = new List<CarsQuantityGroupedByCarTypeDTO>();
		
		string query = @"use car_rental_db;
			select ROW_NUMBER() OVER (order by (select null)) AS id, car_types.car_type_ukr, Count(cars.car_id) as car_quantity
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";
			
			query += this.CreateConditionString(parametersMap);
			query += " group by car_types.car_type_ukr";
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarsQuantityGroupedByCarTypeDTO carsQuantityGroupedByCarTypeDTO = new CarsQuantityGroupedByCarTypeDTO()
						{
							Id = int.Parse(reader["id"].ToString()),
							CarType = reader["car_type_ukr"].ToString(),
							CarsQuantity = int.Parse(reader["car_quantity"].ToString()),
						};

						carsQuantityGroupedByCarTypes.Add(carsQuantityGroupedByCarTypeDTO);
					}
				}
			}
		}

		return carsQuantityGroupedByCarTypes;
	}

	public async Task<List<CarsQuantityGroupedByCarEngineTypeDTO>> GetCarsQuantityGroupedByCarEngineTypesAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken)
	{
		List<CarsQuantityGroupedByCarEngineTypeDTO> carsQuantityGroupedByCarEngineTypes = new List<CarsQuantityGroupedByCarEngineTypeDTO>();
		
		string query = @"use car_rental_db;
			select ROW_NUMBER() OVER (order by (select null)) AS id, car_engine_types.car_engine_type_ukr, Count(cars.car_id) as car_quantity
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";

		query += this.CreateConditionString(parametersMap);
		query += " group by car_engine_types.car_engine_type_ukr";
		
		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarsQuantityGroupedByCarEngineTypeDTO carsQuantityGroupedByCarEngineTypeDTO = new CarsQuantityGroupedByCarEngineTypeDTO()
						{
							Id = int.Parse(reader["id"].ToString()),
							CarEngineType = reader["car_engine_type_ukr"].ToString(),
							CarsQuantity = int.Parse(reader["car_quantity"].ToString()),
						};
						
						carsQuantityGroupedByCarEngineTypes.Add(carsQuantityGroupedByCarEngineTypeDTO);
					}
				}
			}
		}

		return carsQuantityGroupedByCarEngineTypes;
	}

	public async Task<List<CarsQuantityGroupedByYearOfProductionDTO>> GetCarsQuantityGroupedByDateOfProductionAsync(Dictionary<string, StringValues> parametersMap, CancellationToken cancellationToken)
	{
		List<CarsQuantityGroupedByYearOfProductionDTO> carsQuantityGroupedByDateOfProduction = new List<CarsQuantityGroupedByYearOfProductionDTO>();
		
		string query = @"use car_rental_db;
			SELECT ROW_NUMBER() OVER (order by (select null)) AS id, Year(cars.date_of_production) as year_of_production, Count(cars.car_id) as car_quantity
			from cars
			left join car_brands on cars.car_brand_id = car_brands.car_brand_id
			left join car_engine_types on cars.car_engine_type_id = car_engine_types.car_engine_type_id
			left join car_types on cars.car_type_id = car_types.car_type_id";
			
		query += this.CreateConditionString(parametersMap);
		query += " group by Year(cars.date_of_production)";

		using (var connection = new SqlConnection(this.connectionString))
		{
			using (var command = new SqlCommand(query, connection))
			{
				await connection.OpenAsync(cancellationToken);

				using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken))
				{
					while (await reader.ReadAsync(cancellationToken))
					{
						CarsQuantityGroupedByYearOfProductionDTO carsQuantityGroupedByYearOfProductionDTO = new CarsQuantityGroupedByYearOfProductionDTO()
						{
							Id = int.Parse(reader["id"].ToString()),
							YearOfProduction = int.Parse(reader["year_of_production"].ToString()),  
							CarsQuantity = int.Parse(reader["car_quantity"].ToString()),
						};
						
						carsQuantityGroupedByDateOfProduction.Add(carsQuantityGroupedByYearOfProductionDTO);
					}
				}
			}
		}

		return carsQuantityGroupedByDateOfProduction;
	}
}
