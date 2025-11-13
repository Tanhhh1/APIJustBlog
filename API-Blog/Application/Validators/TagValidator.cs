using Application.Models.Tag.DTO;
using FluentValidation;

namespace Application.Validators
{
    public class TagValidator : AbstractValidator<TagSaveDTO>
    {
        public TagValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name must not exceed 255 characters.");

            RuleFor(x => x.UrlSlug)
                .NotEmpty().WithMessage("UrlSlug is required.")
                .MaximumLength(450).WithMessage("UrlSlug must not exceed 450 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        }
    }
}
