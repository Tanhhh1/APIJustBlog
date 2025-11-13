using Application.Models.Post.DTO;
using Domain.Entities;
using FluentValidation;


namespace Application.Validators
{
    public class PostValidator : AbstractValidator<PostSaveDTO>
    {
        public PostValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be a positive non-zero value.");

            RuleFor(x => x.ShortDescription)
                .NotEmpty().WithMessage("ShortDescription is required.")
                .MaximumLength(1000).WithMessage("ShortDescription must not exceed 1000 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(10000).WithMessage("Description must not exceed 10000 characters.");

            RuleFor(x => x.Meta)
                .NotEmpty().WithMessage("Meta is required.")
                .MaximumLength(1000).WithMessage("Meta must not exceed 1000 characters.");

            RuleFor(x => x.UrlSlug)
                .NotEmpty().WithMessage("UrlSlug is required.")
                .MaximumLength(450).WithMessage("UrlSlug must not exceed 450 characters.");
        }
    }
}
