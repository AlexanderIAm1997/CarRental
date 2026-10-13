using CarRental.Entities.DTO;
using FluentValidation;

namespace CarRental.Validators.FluentValidation;

public class UserValidator: AbstractValidator<UserDTO>
{
	public UserValidator()
	{
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Будь ласка, вкажіть ім'я")
            .MaximumLength(50)
            .WithMessage("Ім'я не може містити більше 50 символів");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Будь ласка, вкажіть прізвище")
            .MaximumLength(50)
            .WithMessage("Прізвище не може містити більше 50 символів");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120)
            .WithMessage("Вік повинен бути від 18 до 120 років");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Будь ласка, вкажіть номер телефону")
            .Matches(@"^\+380\d{9}$")
            .WithMessage("Номер телефону повинен мати формат +380XXXXXXXXX");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Будь ласка, вкажіть електронну пошту")
            .EmailAddress()
            .WithMessage("Вкажіть коректну електронну пошту")
            .MaximumLength(100)
            .WithMessage("Електронна пошта занадто довга");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Будь ласка, введіть пароль")
            .MinimumLength(8)
            .WithMessage("Пароль повинен містити щонайменше 8 символів")
            .MaximumLength(100)
            .WithMessage("Пароль не може містити більше 100 символів")
            .Matches("[A-Z]")
            .WithMessage("Пароль повинен містити хоча б одну велику літеру")
            .Matches("[a-z]")
            .WithMessage("Пароль повинен містити хоча б одну малу літеру")
            .Matches("[0-9]")
            .WithMessage("Пароль повинен містити хоча б одну цифру");

	}
}	