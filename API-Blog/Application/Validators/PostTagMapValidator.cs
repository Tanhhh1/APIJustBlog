using Application.DTOs.PostTagMap;
using FluentValidation;

namespace Application.Validators
{
    public class PostTagMapValidator : AbstractValidator<PostTagMapSaveDTO>
    {
        public PostTagMapValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("PostId must be a positive non-zero value.");

            RuleFor(x => x.TagIds)
                .NotNull().WithMessage("TagIds is required.")
                .Must(t => t != null && t.Count > 0).WithMessage("At least one TagId is required.");

            RuleForEach(x => x.TagIds)
                .GreaterThan(0).WithMessage("Each TagId must be a positive non-zero value.");
        }
    }
}
