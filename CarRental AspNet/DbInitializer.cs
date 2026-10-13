using System;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

public class DbInitializer
{
	private IConfiguration configuration {get;set;}
	private readonly CancellationTokenSource tokenSource;
	private readonly CancellationToken cancellationToken;
	public DbInitializer(IConfiguration configuration)
	{
		this.configuration = configuration;
		this.tokenSource = new CancellationTokenSource();
		this.cancellationToken = this.tokenSource.Token;
	} 
	public async Task CreateDatabase()
	{
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringBeforeCreatingDatabase"];

		string queryString = $@"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'car_rental_db')
			BEGIN
				CREATE DATABASE car_rental_db;
			END";

		using (SqlConnection connection = new(connectionString))
		{
			SqlCommand command = new(queryString, connection);
			try
			{
				await connection.OpenAsync(cancellationToken);
				await command.ExecuteNonQueryAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

	}
	
	public async Task CreateTables()
	{
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];

		List<string> queries = new List<string>()
		{
			"use car_rental_db",
			@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'users')
			BEGIN
				create table [users] (user_id int identity(1,1) primary key, first_name NVARCHAR(250) not null, last_name NVARCHAR(250) not null, age int not null, email VARCHAR(250) not null, phone NVARCHAR(50) not null, hashed_password NVARCHAR(250) not null, created_at datetime2, role_id int not null);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'cars')
			BEGIN
				create table cars(car_id int identity(1,1) primary key, car_brand_id int not null, model NVARCHAR(250) not null, car_type_id int NOT NULL,  car_engine_type_id int not null, date_of_production date not null, description_eng NVARCHAR(250) NOT NULL, description_ukr NVARCHAR(250) NOT NULL, mileage int NOT NULL, is_available BIT NOT NULL DEFAULT 1, price_per_day decimal NOT NULL);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'car_types')
			BEGIN
				create table car_types(car_type_id int identity(1,1) primary key, car_type_eng NVARCHAR(250), car_type_ukr NVARCHAR(250) not null);                     
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'car_brands')
			BEGIN
				create table car_brands(car_brand_id int identity(1,1) primary key, car_brand NVARCHAR(250));                     
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'car_engine_types')
			BEGIN
				create table car_engine_types (car_engine_type_id int identity(1,1) primary key, car_engine_type_eng NVARCHAR(250), car_engine_type_ukr NVARCHAR(250) not null);                     
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'payments')
			BEGIN
				create table payments(payment_id int identity(1,1) primary key, user_id int not null, car_id int not null, amount int not null, status BIT NOT NULL DEFAULT 0, paid_at date not null);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'rentals')
			BEGIN
				create table rentals(rental_id int identity(1,1) primary key, user_id int not null, car_id int not null, start_date date not null, end_date date not null, mileage int not null, payment_id int not null);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'feedbacks')
			BEGIN
				create table feedbacks(feedback_id int identity(1,1) primary key, user_id int not null, car_id int not null, content NVARCHAR(250), rate int, created_at date not null);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'roles')
			BEGIN
				CREATE TABLE roles(role_id INT PRIMARY KEY IDENTITY(1,1), role NVARCHAR(50) NOT NULL UNIQUE);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'picture_links')
			BEGIN
				CREATE TABLE picture_links(picture_link_id INT PRIMARY KEY IDENTITY(1,1), user_id int NOT NULL, picture_link NVARCHAR(250) not null);
			END;",
			$@"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'refresh_tokens')
			BEGIN
				CREATE TABLE refresh_tokens(refresh_token_id INT PRIMARY KEY IDENTITY(1,1), user_id int NOT NULL, refresh_token VARCHAR(250) not null, issued_at datetime2 not null, expires_at datetime2 not null);
			END;",
		};
		
		foreach(string query in queries)
		{
			using(SqlConnection connection = new(connectionString))
			{
				using (SqlCommand command = new(query, connection))
				{
					try
					{
						await connection.OpenAsync(cancellationToken);
						await command.ExecuteNonQueryAsync(cancellationToken);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}	
				}
			}
		}

		return;
	}	
	public async Task AddConstraints()
	{
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];

		List<string> constraints = new List<string>()
		{
			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_user_role_id')
			BEGIN
				ALTER TABLE [users]
				ADD CONSTRAINT fk_user_role_id
				FOREIGN KEY (role_id)
				REFERENCES roles (role_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_car_brand_id')
			BEGIN
				ALTER TABLE cars
				ADD CONSTRAINT fk_car_brand_id
				FOREIGN KEY (car_brand_id)
				REFERENCES car_brands (car_brand_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_car_type_id')
			BEGIN
				ALTER TABLE cars
				ADD CONSTRAINT fk_car_type_id
				FOREIGN KEY (car_type_id)
				REFERENCES car_types (car_type_id);
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_car_engine_type_id')
			BEGIN
				ALTER TABLE cars
				ADD CONSTRAINT fk_car_engine_type_id
				FOREIGN KEY (car_engine_type_id)
				REFERENCES car_engine_types (car_engine_type_id);
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_payment_user_id')
			BEGIN
				ALTER TABLE payments
				ADD CONSTRAINT fk_payment_user_id
				FOREIGN KEY (user_id)
				REFERENCES [users] (user_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_payment_car_id')
			BEGIN
				ALTER TABLE payments
				ADD CONSTRAINT fk_payment_car_id
				FOREIGN KEY (car_id)
				REFERENCES cars (car_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_rental_payment_id')
			BEGIN
				ALTER TABLE rentals
				ADD CONSTRAINT fk_rental_payment_id
				FOREIGN KEY (payment_id)
				REFERENCES payments (payment_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_rental_car_id')
			BEGIN
				ALTER TABLE rentals
				ADD CONSTRAINT fk_rental_car_id
				FOREIGN KEY (car_id)
				REFERENCES cars (car_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_rental_user_id')
			BEGIN
				alter table rentals
				add constraint fk_rental_user_id
				FOREIGN KEY (user_id)
				REFERENCES [users] (user_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_feedback_user_id')
			BEGIN
				ALTER TABLE feedbacks
				ADD CONSTRAINT fk_feedback_user_id
				FOREIGN KEY (user_id)
				REFERENCES [users] (user_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_feedback_car_id')
			BEGIN
				ALTER TABLE feedbacks
				ADD CONSTRAINT fk_feedback_car_id
				FOREIGN KEY (car_id)
				REFERENCES cars (car_id)
			END",

			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_picture_links_user_id')
			BEGIN
				ALTER TABLE picture_links
				ADD CONSTRAINT fk_picture_links_user_id
				FOREIGN KEY (user_id)
				REFERENCES [users] (user_id)
			END",
			
			@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'fk_refresh_tokens_user_id')
			BEGIN
				ALTER TABLE refresh_tokens
				ADD CONSTRAINT fk_refresh_tokens_user_id
				FOREIGN KEY (user_id)
				REFERENCES [users] (user_id)
			END",
		};

		foreach(string query in constraints)
		{
			using(SqlConnection connection = new(connectionString))
			{
				using (SqlCommand command = new(query, connection))
				{
					try
					{
						await connection.OpenAsync(cancellationToken);
						await command.ExecuteNonQueryAsync(cancellationToken);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}	
				}
			}
		}
	}

	public async Task SeedRolesAsync()
	{
		string[] roles = new string[]
		{
			$@"IF NOT EXISTS(SELECT * FROM roles WHERE role = 'Admin')
			BEGIN
				insert into roles(role) values('Admin');
			END;", 
			$@"IF NOT EXISTS(SELECT * FROM roles WHERE role = 'User')
			BEGIN
				insert into roles(role) values('User');
			END;",
		};

		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		
		foreach(string query in roles)
		{	
			using(SqlConnection connection = new(connectionString))
			{
				SqlCommand command = new(query, connection);
				try
				{
					await connection.OpenAsync(cancellationToken);
					await command.ExecuteNonQueryAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}

	public async Task SeedCarBrandsAsync()
	{
		string[] carBrands = new string[]
		{
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Toyota')
			BEGIN
				insert into car_brands(car_brand) values('Toyota');
			END;", 
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Chevrole')
			BEGIN
				insert into car_brands(car_brand) values('Chevrole');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Ford')
			BEGIN
				insert into car_brands(car_brand) values('Ford');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Geely')
			BEGIN
				insert into car_brands(car_brand) values('Geely');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Renault')
			BEGIN
				insert into car_brands(car_brand) values('Renault');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Peugeot')
			BEGIN
				insert into car_brands(car_brand) values('Peugeot');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Citroen')
			BEGIN
				insert into car_brands(car_brand) values('Citroen');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'BMW')
			BEGIN
				insert into car_brands(car_brand) values('BMW');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Volkswagen')
			BEGIN
				insert into car_brands(car_brand) values('Volkswagen');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Opel')
			BEGIN
				insert into car_brands(car_brand) values('Opel');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Mazda')
			BEGIN
				insert into car_brands(car_brand) values('Mazda');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Honda')
			BEGIN
				insert into car_brands(car_brand) values('Honda');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Hyunday')
			BEGIN
				insert into car_brands(car_brand) values('Hyunday');
			END;",
			$@"IF NOT EXISTS(SELECT car_brand FROM car_brands WHERE car_brand = 'Kia')
			BEGIN
				insert into car_brands(car_brand) values('Kia');
			END;",
		};

		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		
		foreach(string query in carBrands)
		{
			
			using(SqlConnection connection = new(connectionString))
			{
				using (SqlCommand command = new(query, connection))
				{
					try
					{
						await connection.OpenAsync(cancellationToken);
						await command.ExecuteNonQueryAsync(cancellationToken);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}	
				}
			}
		}
	}
	public async Task SeedCarTypesAsync()
	{
		string[] carTypes = new string[]
		{
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Хетчбек' or car_type_eng = 'Hatchback')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Хетчбек', 'Hatchback');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Универсал' or car_type_eng = 'Station wagon')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Универсал', 'Station wagon');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Купе' or car_type_eng = 'Coupe')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Купе', 'Coupe');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Кроссовер' or car_type_eng = 'Crossover')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values (N'Кроссовер', 'Crossover');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Позашляховик' or car_type_eng = 'SUV')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Позашляховик', 'SUV');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Пікап' or car_type_eng = 'Pickup')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Пікап', 'Pickup');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Мінівен' or car_type_eng = 'Minivan')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Мінівен', 'Minivan');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Кабріолет' or car_type_eng = 'Convertible')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Кабріолет', 'Convertible');
			END;",
			$@"IF NOT EXISTS(SELECT car_type_ukr, car_type_eng FROM car_types WHERE car_type_ukr = 'Родстер' or car_type_eng = 'Roadster')
			BEGIN
				insert into car_types(car_type_ukr, car_type_eng) values(N'Родстер', 'Roadster');
			END;",
		};
 
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		
		foreach(string query in carTypes)
		{
			using(SqlConnection connection = new(connectionString))
			{
				SqlCommand command = new(query, connection);
				try
				{
					await connection.OpenAsync(cancellationToken);
					await command.ExecuteNonQueryAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}

	public async Task SeedCarEngineTypes()
	{
		string[] carEngineTypes = new string[]
		{
			$@"IF NOT EXISTS(SELECT car_engine_type_ukr, car_engine_type_eng FROM car_engine_types WHERE car_engine_type_ukr = 'Бензиновий' or car_engine_type_eng = 'Petrol')
			BEGIN
				insert into car_engine_types(car_engine_type_ukr, car_engine_type_eng) values (N'Бензиновий', 'Petrol');
			END;",
			$@"IF NOT EXISTS(SELECT car_engine_type_ukr, car_engine_type_eng FROM car_engine_types WHERE car_engine_type_ukr = 'Дизельний' or car_engine_type_eng = 'Diesel')
			BEGIN
				insert into car_engine_types(car_engine_type_ukr, car_engine_type_eng) values(N'Дизельний', 'Diesel');
			END;",
			$@"IF NOT EXISTS(SELECT car_engine_type_ukr, car_engine_type_eng FROM car_engine_types WHERE car_engine_type_ukr = 'Електричний' or car_engine_type_eng = 'Electric')
			BEGIN
				insert into car_engine_types(car_engine_type_ukr, car_engine_type_eng) values(N'Електричний', 'Electric');
			END;",
			$@"IF NOT EXISTS(SELECT car_engine_type_ukr, car_engine_type_eng FROM car_engine_types WHERE car_engine_type_ukr = 'Гібридний' or car_engine_type_eng = 'Hybrid')
			BEGIN
				insert into car_engine_types(car_engine_type_ukr, car_engine_type_eng) values(N'Гібридний', 'Hybrid');
			END;",
		};
 
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		
		foreach(string query in carEngineTypes)
		{
			using(SqlConnection connection = new(connectionString))
			{
				SqlCommand command = new(query, connection);
				try
				{
					await connection.OpenAsync(cancellationToken);
					await command.ExecuteNonQueryAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}	
	} 

	public async Task SeedCars()
	{
		string[] cars = new string[]
		{
			$@"IF NOT EXISTS(SELECT * FROM cars)
			BEGIN
				INSERT INTO cars (car_brand_id, model, car_engine_type_id, date_of_production, description_eng, description_ukr, car_type_id, mileage, is_available, price_per_day)
				values(1,  N'Corolla',  1, '2022-03-15', N'Reliable and fuel-efficient compact hatchback', N'Надійний та економічний компактний хетчбек', 1, 45000, 1, 45), 
					(8, N'X5', 2, '2021-08-20', N'Premium luxury SUV with powerful diesel engine', N'Преміальний люксовий позашляховик з потужним дизельним двигуном', 5, 62000, 1, 120),
					(13, N'Ioniq 5', 3, '2023-01-10', N'Modern all-electric crossover with long range', N'Сучасний повністю електричний кросовер з великим запасом ходу',   4, 18000, 1, 85),
					(3,  N'Ranger', 2, '2020-11-05', N'Rugged mid-size pickup truck perfect for work and adventure', N'Міцний середньорозмірний пікап ідеальний для роботи та пригод', 6, 78000, 0, 70),				
					(12, N'CR-V', 4, '2022-06-18', N'Spacious hybrid crossover with excellent fuel economy', N'Просторий гібридний кросовер з відмінною економічністю', 4, 35000, 1, 65),
					(9,  N'Golf',1, '2021-05-12', N'Popular compact hatchback with excellent handling', N'Популярний компактний хетчбек з відмінною керованістю', 1, 52000, 1, 50),
					(2,  N'Tahoe', 2, '2020-09-30', N'Full-size SUV with powerful diesel engine and high capacity', N'Повнорозмірний позашляховик з потужним дизелем та великою місткістю', 5, 89000, 1, 110),
					(11, N'CX-5', 1, '2022-04-18', N'Stylish crossover with sporty dynamics and comfort', N'Стильний кросовер зі спортивною динамікою та комфортом', 4, 31000, 1, 60),
					(5,  N'Megane', 1, '2019-11-22', N'Practical hatchback with modern design and efficiency', N'Практичний хетчбек із сучасним дизайном та економічністю', 1, 67000, 0, 40),
					(14, N'Sportage', 4, '2023-02-14', N'Popular hybrid crossover with advanced safety features', N'Популярний гібридний кросовер з передовими системами безпеки', 4, 15000, 1, 75),
					(8,  N'M4', 1, '2021-07-08', N'High-performance coupe with powerful petrol engine', N'Високопродуктивне купе з потужним бензиновим двигуном', 3, 28000, 1, 150),
					(6,  N'3008', 2, '2020-12-03', N'Comfortable crossover with efficient diesel engine', N'Комфортний кросовер з ефективним дизельним двигуном', 4, 74000, 1, 55),
					(1,  N'RAV4', 4, '2022-08-25', N'Reliable hybrid SUV with excellent off-road capability', N'Надійний гібридний позашляховик з відмінною прохідністю', 5, 39000, 1, 80),
					(3,  N'Mustang', 1, '2019-06-17', N'Iconic American muscle coupe with thrilling performance', N'Іконічне американське м’язисте купе з захоплюючою продуктивністю', 3, 55000, 0, 130),
					(12, N'Civic', 1, '2021-10-09', N'Sporty and efficient hatchback for everyday driving', N'Спортивний та економічний хетчбек для щоденної їзди', 1, 48000, 1, 48),
					(10, N'Insignia', 2, '2018-03-28', N'Spacious station wagon with comfortable diesel engine', N'Просторий універсал з комфортним дизельним двигуном', 2, 112000, 1, 45),
					(4,  N'Coolray', 1, '2022-11-15', N'Affordable modern crossover with turbo petrol engine', N'Доступний сучасний кросовер з турбованим бензиновим двигуном', 4, 22000, 1, 42),
					(7,  N'C5 Aircross', 2, '2021-01-20', N'Comfort-oriented crossover with smooth diesel powertrain', N'Комфорт-орієнтований кросовер з плавним дизельним силовим агрегатом', 4, 58000, 1, 58),
					(9,  N'Tiguan', 1, '2020-07-11', N'Versatile family crossover with solid petrol engine', N'Універсальний сімейний кросовер з надійним бензиновим двигуном', 4, 71000, 0, 62),
					(13, N'Tucson', 4, '2023-05-03', N'Modern hybrid SUV with stylish design and great efficiency', N'Сучасний гібридний позашляховик зі стильним дизайном та економічністю',   5, 12000, 1, 78),
					(2,  N'Camaro', 1, '2019-04-26', N'Powerful sports coupe with aggressive styling', N'Потужне спортивне купе з агресивним стилем', 3, 61000, 1, 125),
					(11, N'MX-5', 1, '2021-09-14', N'Classic roadster with pure driving pleasure', N'Класичний родстер з чистим задоволенням від водіння', 9, 24000, 1, 95),
					(14, N'Sorento', 2, '2020-02-19', N'Spacious family SUV with reliable diesel engine', N'Просторий сімейний позашляховик з надійним дизельним двигуном', 5, 83000, 1, 70),
					(5,  N'Kadjar', 1, '2019-08-07', N'Practical crossover with good ground clearance', N'Практичний кросовер з хорошим дорожнім просвітом', 4, 95000, 0, 48)
			END;"
		};
 
		string connectionString = configuration.GetSection("ConnectionStrings")["ConnectionStringAfterCreatingDatabase"];
		
		foreach(string query in cars)
		{
			using(SqlConnection connection = new(connectionString))
			{
				SqlCommand command = new(query, connection);
				try
				{
					await connection.OpenAsync(cancellationToken);
					await command.ExecuteNonQueryAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}	
	}
}

