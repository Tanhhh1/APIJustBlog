using Application.Models.Auth.DTO;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpDTO>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username must not be empty");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP must not be empty");
        }
    }

}
