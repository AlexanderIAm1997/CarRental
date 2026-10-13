using CarRental.Entities.DTO;
using FluentValidation;

namespace CarRental.Validators.FluentValidation;
public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
{
	public UserLoginDTOValidator()
	{
		 RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Будь ласка, вкажіть електронну пошту")
            .EmailAddress()
            .WithMessage("Вкажіть коректний логін")
            .MaximumLength(100)
            .WithMessage("логін занадто довгий");

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