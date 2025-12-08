using FluentValidation;
using Application.Models.Auth.DTO;

namespace Application.Validators.Auth
{
    public class SignInValidator : AbstractValidator<SignInDTO>
    {
        public SignInValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
